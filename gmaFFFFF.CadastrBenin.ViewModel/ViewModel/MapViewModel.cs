using System;
using System.Collections.ObjectModel;
using gmaFFFFF.CadastrBenin.DAL;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading;
using Microsoft.Maps.MapControl.WPF;
using DotSpatial.Projections;
using DotSpatial.Topology.Utilities;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.Command;


namespace gmaFFFFF.CadastrBenin.ViewModel
{
	/// <summary>
	/// Модель представления для окна поиска на карте
	/// </summary>
	public class MapViewModel : MyViewModelBase
	{
		/// <summary>
		/// Контекст базы данных
		/// </summary>
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
				LoadDataAsync();
			}

			//Инициализация комманд
			FindParcelCommand = new RelayCommand<string>(FindParcelAsync, (o) => FindParcelCommandCanExecute);
			FindParcelCommandCanExecute = true;
			ClearFindedParcelCommand = new RelayCommand(ClearFindedParcel);
		}
		/// <summary>
		/// Перепроецирует геометрию в WGS84 и возвращает набор точек
		/// </summary>
		/// <param name="input">Простые полигоны и полилинии из БД в системе координать UtmZone31N</param>
		/// <returns>Набор точек в системе координат WGS84 (SRID 4326)</returns>
		/// <remarks>Внимание! Многоконтурная геометрия не поддерживается</remarks>
		public LocationCollection UtmZone31nToWgs84Transform(DbGeometry input)
		{
			if (input == null)
				return null;

			//Перевод геометрии в формат понятный для библиотеки DotSpatial
			byte[] wkbBynary = input.AsBinary();
			WkbReader wkbReader = new WkbReader();
			DotSpatial.Topology.IGeometry geom = wkbReader.Read(wkbBynary);

			//Определение используемых проекций
			ProjectionInfo beninPrj = KnownCoordinateSystems.Projected.UtmWgs1984.WGS1984UTMZone31N;
			ProjectionInfo worldPrj = KnownCoordinateSystems.Geographic.World.WGS1984;

			//Создание массива точек Х1,У1,Х2,У2...Хn,Уn
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

			//Операция перепроецирования над массивом точек
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
				FindParcelCommandCanExecute = false;//Отключаем команду поиска
				//Создаем токен отмены
				CancelFindParcel = new CancellationTokenSource();
				var cancelToken = CancelFindParcel.Token;

				using (var FindContextDB = ContextDbFactory.Create())
				{
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
						.OrderBy(p=>p.Parcel.NUP)
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

					//Выбираем все здания связанные с земельными участками и проецируем их во внутреннее представление
					var buildings = (from bl in parcels
						from b in bl.BuildingsOnParcel
						select b).ToList();

					//Если поиск не был отменен, то представляем его результаты
					cancelToken.ThrowIfCancellationRequested();
					FindParcels = new ObservableCollection<ParcelModel>(parcels);
					FindBuildings = new ObservableCollection<BuildingOnParcelModel>(buildings);
				}
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
		/// <summary>
		/// Очищает список найденных земельных участков и связанных с ними зданий
		/// </summary>
		protected void ClearFindedParcel()
		{
			FindParcels.Clear();
			FindBuildings.Clear();
			RaisePropertyChanged(nameof(FindParcels));
			RaisePropertyChanged(nameof(FindBuildings));
		}

		#region Публичные поля для привязки данных
		/// <summary>
		/// Кадастровое деление - департаменты
		/// </summary>
		public ObservableCollection<LocationCollection> CadDepartamentsLocations { get; set; }
		/// <summary>
		/// Кадастровое деление - зоны
		/// </summary>
		public ObservableCollection<LocationCollection> CadZoneLocations { get; set; }
		/// <summary>
		/// Кадастровое деление - районы
		/// </summary>
		public ObservableCollection<LocationCollection> CadSectorLocations { get; set; }
		/// <summary>
		/// Земельные участки, найденные операцией поиска
		/// </summary>
		public ObservableCollection<ParcelModel> FindParcels { get; set; }
		/// <summary>
		/// Здания, найденные во время поиска земельных участков и расположенные на найденных земельных участках
		/// </summary>
		public ObservableCollection<BuildingOnParcelModel> FindBuildings { get; set; }
		/// <summary>
		/// Максимальное число найденных земельных участков
		/// </summary>
		public int FindedParcelsMaxCount { get; set; } = 50;
		/// <summary>
		/// Токен для отмены операции поиска
		/// </summary>
		public CancellationTokenSource CancelFindParcel = null;
		#endregion

		#region Команды
		/// <summary>
		/// Команда найти земельный участок
		/// </summary>
		public RelayCommand<string> FindParcelCommand { get; set; }
		private bool _findParcelCommandCanExecute;
		/// <summary>
		/// Возможно ли выполнить поиск земельного участка
		/// </summary>
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
		/// <summary>
		/// Команда очистить найденные земельные участки и связанные с ними здания
		/// </summary>
		public RelayCommand ClearFindedParcelCommand { get; set; }

		#endregion

	}
}


