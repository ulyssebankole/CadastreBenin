using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gmaFFFFF.CadastrBenin.DAL;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.SqlServer.Types;
using DotSpatial.Projections;
using DotSpatial.Topology.Utilities;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.Command;


namespace gmaFFFFF.CadastrBenin.ViewModel
{
	public class MapViewModel : ViewModelBase
	{
		/// <summary>
		/// Контекст базы данных
		/// </summary>
		private CadastrBeninDB FindContextDB;
		private DBContextFactory ContextDbFactory;
		public MapViewModel(DBContextFactory contextFactory)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				ContextDbFactory = contextFactory;
				FindContextDB = ContextDbFactory.Create();
				FindContextDB.Database.CommandTimeout = 60;
				LoadDataAsync();
			}

			//Инициализация комманд
			FindParcelCommand = new RelayCommand<string>(FindParcelAsync, (o) => FindParcelCommandCanExecute);
			FindParcelCommandCanExecute = true;
			ClearFindedParcelCommand = new RelayCommand(ClearFindedParcel);
		}

		public override void Cleanup()
		{
			FindContextDB.Dispose();
			base.Cleanup();
		}

		/// <summary>
		/// Перепроецирует геометрию в WGS84 и возвращает набор точек
		/// </summary>
		/// <param name="input">Простые полигоны и полилинии из БД в системе координать UtmZone31N</param>
		/// <returns>Набор точек в системе координат WGS84 (SRID 4326)</returns>
		public LocationCollection UtmZone31nToWgs84Transform(DbGeometry input)
		{
			if (input == null)
				return null;
			
			byte[] wkbBynary = input.AsBinary();

			
			WkbReader wkbReader = new WkbReader();

			ProjectionInfo beninPrj = KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone31N;
			ProjectionInfo worldPrj = KnownCoordinateSystems.Geographic.World.WGS1984;
			DotSpatial.Topology.IGeometry geom = wkbReader.Read(wkbBynary);


			double[] pointArray = new double[geom.Coordinates.Count() * 2];
			double[] zArray = new double[1];
			zArray[0] = 0;

			int counterX = 0; int counterY = 1;
			foreach (var coordinate in geom.Coordinates)
			{
				pointArray[counterX] = coordinate.X;
				pointArray[counterY] = coordinate.Y;

				counterX += 2;
				counterY += 2;
			}

			Reproject.ReprojectPoints(pointArray, zArray, beninPrj, worldPrj, 0, (pointArray.Length / 2));

			LocationCollection loc = new LocationCollection();
			counterX = 0; counterY = 1;
			
			foreach (var coordinate in geom.Coordinates)
			{
				loc.Add(new Location(pointArray[counterY], pointArray[counterX]));
 
				 counterX += 2;
				 counterY += 2;
			}
			
			return loc;
		}

		
		/// <summary>
		/// Загружает данные из базы данных
		/// </summary>
		async protected void LoadDataAsync()
		{
			using (var ContextDb = ContextDbFactory.Create())
			{
				var dbDep = await ContextDb.CadastraleDepartements_v.Select((g) => g.Shape).ToListAsync();
				var dbZone = await ContextDb.CadastraleZones_v.Select((g) => g.Shape).ToListAsync();
				var dbSect = await ContextDb.CadastraleSecteurs_v.Select((g) => g.Shape).ToListAsync();

				var depLoc = dbDep.AsParallel().Select((s) => UtmZone31nToWgs84Transform(s)).ToList();
				var zoneLoc = dbZone.AsParallel().Select((s) => UtmZone31nToWgs84Transform(s)).ToList();
				var sectLoc = dbSect.AsParallel().Select((s) => UtmZone31nToWgs84Transform(s)).ToList();

				CadDepartamentsLocations = new ObservableCollection<LocationCollection>(depLoc);
				CadZoneLocations = new ObservableCollection<LocationCollection>(zoneLoc);
				CadSectorLocations = new ObservableCollection<LocationCollection>(sectLoc);
			}
			//Направляем уведомление об изменении свойств
			RaisePropertyChanged(nameof(CadDepartamentsLocations));
			RaisePropertyChanged(nameof(CadZoneLocations));
			RaisePropertyChanged(nameof(CadSectorLocations));
		}
		/// <summary>
		/// Осуществляет поиск земельных участков, уникальный номер которых начинается с заданных символов
		/// </summary>
		/// <remarks>Результаты операции сохраняется в <see cref="FindParcels"/> и в <see cref="FindBuildings"/>.
		/// Функция не допускает повторынй вызов, до завершения предыдущей операции </remarks>
		/// <param name="num">Начальные символы уникального номера земельных участков</param>
		async protected void FindParcelAsync(string num)
		{
			if (CancelFindParcel != null)           //Не разрешаем повторное выполнение асинхронной операции,
				return;								//если предыдущая не завершилась
			
			try
			{
				FindParcelCommandCanExecute = false;
				CancelFindParcel = new CancellationTokenSource();
				var cancelToken = CancelFindParcel.Token;

				//Подготовка запроса к БД
				var ParcelFullDB = FindContextDB
					.JuridiqueObjets.OfType<Parcelle>()
					.Include(i => i.Impots).Include(ut => ut.Utilisations)
					.GroupJoin(FindContextDB.Sinestree_Parcelles_v, p => p.NUP, s => s.NUP,
						(p, ss) => new {Parcel = p, Sinistrees = ss})
					.GroupJoin(FindContextDB.JuridiqueSituations_v, p => p.Parcel.NUP, j => j.NUP,
						(p, js) => new {Parcel = p.Parcel, Sinistrees = p.Sinistrees, Rights = js})
					.GroupJoin(FindContextDB.Immeubles_Parcelles_v, p => p.Parcel.NUP, i => i.ParcelNUP,
						(p, i) =>
							new {Parcel = p.Parcel, Sinistrees = p.Sinistrees, Rights = p.Rights, BuildingCount = i.FirstOrDefault()})
					.GroupJoin(FindContextDB.Parcel_ImmeubleIntersect, p => p.Parcel.NUP, i => i.ParcelNUP,
						(p, i) =>
							new
							{
								Parcel = p.Parcel,
								Sinistrees = p.Sinistrees,
								Rights = p.Rights,
								BuildingCount = p.BuildingCount,
								Buildings = i
							});

				//Выполняем запрос к БД
				var parcelsDB = await ParcelFullDB.Where(p => p.Parcel.NUP.StartsWith(num))
					.Take(FindedParcelsMaxCount)
					.ToListAsync(cancelToken);

				//Проецируем результат запроса к БД во внутреннее представление земельных участков
				var parcels = parcelsDB.AsParallel().Select(p => new ParcelModel
				{
					Parcel = p.Parcel
					,WgsLocation = UtmZone31nToWgs84Transform(p.Parcel.Shape)
					,Centroid = UtmZone31nToWgs84Transform(p.Parcel.Shape.Centroid).First()
					,Sinistrees = new ObservableCollection<Sinestree_Parcelles_v>(p.Sinistrees)
					,Rights = new ObservableCollection<JuridiqueSituations_v>(p.Rights)
					,BuildingCount = p.BuildingCount.ImmeublesCount
					,BuildingsOnParcel = new ObservableCollection<BuildingOnParcelModel>(p.Buildings
						.Select(b => new BuildingOnParcelModel()
						{
							Building = b
							,Centroid = UtmZone31nToWgs84Transform(b.ImmeubleShape.Centroid).First()
							,WgsLocation = UtmZone31nToWgs84Transform(b.ImmeubleShape)
						}))
				}).ToList();

				//Выбираем все здания связанные с земельными участками и проецируем их во внутренне представление
				var buildings = (from bl in parcels
					from b in bl.BuildingsOnParcel
					select b).ToList();

				//Если поиск не был отменен, то представляем его результаты
				cancelToken.ThrowIfCancellationRequested();
				FindParcels = new ObservableCollection<ParcelModel>(parcels);
				FindBuildings = new ObservableCollection<BuildingOnParcelModel>(buildings);

				RaisePropertyChanged(nameof(FindParcels));
				RaisePropertyChanged(nameof(FindBuildings));
			}
			catch (Exception e)
			{
				if ((e is OperationCanceledException) || (e.InnerException is OperationCanceledException))
					System.Diagnostics.Debug.Print("Отменена асинхронная операция FindParcelAsync.");
				else
					throw;
			}
			finally
			{
				FindParcelCommandCanExecute = true;	//Запуск поиска разрешен
				CancelFindParcel = null;			//Очищаем токен асинхронной операции
			}
		}

		protected void ClearFindedParcel()
		{
			FindParcels.Clear();
			FindBuildings.Clear();
			RaisePropertyChanged(nameof(FindParcels));
			RaisePropertyChanged(nameof(FindBuildings));
		}

		private CancellationTokenSource CancelFindParcel = null;


		#region Публичные поля для привязки данных
		public ObservableCollection<LocationCollection> CadDepartamentsLocations { get; set; }
		public ObservableCollection<LocationCollection> CadZoneLocations { get; set; }
		public ObservableCollection<LocationCollection> CadSectorLocations { get; set; }
		public ObservableCollection<ParcelModel> FindParcels { get; set; }
		public ObservableCollection<BuildingOnParcelModel> FindBuildings { get; set; }
		public int FindedParcelsMaxCount { get; set; } = 25;

		#endregion
		#region Команды
		public RelayCommand<string> FindParcelCommand { get; set; }

		private bool _findParcelCommandCanExecute;
		public bool FindParcelCommandCanExecute
		{
			get { return _findParcelCommandCanExecute; }
			set
			{
				if (_findParcelCommandCanExecute = value)
					return;
				_findParcelCommandCanExecute = value;
				FindParcelCommand.RaiseCanExecuteChanged();
			}
		}
		public RelayCommand ClearFindedParcelCommand { get; set; }
		public RoutedUICommand SaveChangesCommand { get; set; } = ApplicationCommands.Save;
		public RoutedUICommand UndoChangesCommand { get; set; } = ApplicationCommands.Undo;

		#endregion

	}
}


