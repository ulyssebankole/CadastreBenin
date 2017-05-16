using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using gmaFFFFF.CadastrBenin.DAL;
using gmaFFFFF.CadastrBenin.ViewModel.Services;
using gmaFFFFF.CadastrBenin.ViewModel.Message;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	public class ParcelEditViewModel : MyViewModelBase
	{
		/// <summary>
		/// Контекст базы данных
		/// </summary>
		private CadastrBeninDB ContextDb;
		/// <summary>
		/// Фабрика для создания контекстов данных
		/// </summary>
		private DBContextFactory ContextFactory;
		/// <summary>
		/// Выполняемая задача загрузки данных
		/// </summary>
		private Task LoadData;
		/// <summary>
		/// Выполняемая задача обновления данных
		/// </summary>
		private Task RefreshData;
		/// <summary>
		/// Кэш для отслеживания удаленных земельных участков
		/// </summary>
		private ObservableCollection<Parcelle> ParcelsDelete { get; set; } = new ObservableCollection<Parcelle>();
		

		public ParcelEditViewModel(DBContextFactory contextFactory)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				ContextFactory = contextFactory;
				ContextDb = ContextFactory.Create();
				LoadData = LoadDataAsync();
			}

			//Инициализация комманд
			SaveChangesCommand = new RelayCommand(SaveChanges);
			UndoChangesCommand = new RelayCommand(UndoChanges);

			//Подписка на уведомление об изменении справочников
			Messenger.Default.Register<ReferenceRefreshedMessage>(this, async (msg) =>
			{
				if (LoadData != null)					//Если не завершилась первичная загрузка данных, то ждем ее
					await Task.WhenAll(LoadData);
				if (RefreshData != null)                //Если не завершилась предыдущая операция обновления данных, то ждем ее
					await Task.WhenAll(RefreshData);
				RefreshData = RefreshDataAsync();		//Запускаем операцию обновления данных
			});
		}
		/// <summary>
		/// Очистка неуправляемых ресурсов
		/// </summary>
		public override void Cleanup()
		{
			ContextDb.Dispose();
			base.Cleanup();
		}

		/// <summary>
		/// Сохраняет все внесенные изменения
		/// </summary>
		protected void SaveChanges()
		{
			if (!SaveUndoCommandCanExecute)
				return;
			
			//Помечаем к удалению записи, которые больше не связаны с земельным участком 
			DeleteDetailRecordTogetherRelationship();

			ContextDb.SaveChanges();

			//Удаленные участки больше не нужны в кэше
			ParcelsDelete.Clear();
		}
		/// <summary>
		/// Отменяет все внесенные изменения
		/// </summary>
		protected void UndoChanges()
		{
			if (!SaveUndoCommandCanExecute)
				return;
			
			//Помечаем к удалению записи, которые больше не связаны с земельным участком 
			DeleteDetailRecordTogetherRelationship();

			//Удаляем из кэша участков добавленные в данном сеансе редактирования
			var addParcel = ContextDb.ChangeTracker.Entries<Parcelle>().Where(p => p.State == EntityState.Added);
			foreach (var del in addParcel)
			{
				Parcels.Remove(del.Entity);
			}

			//Отменяем все изменения в БД
			ContextDb.DiscardAllChanges();
			

			//Возвращаем из кэша удаленные земельные участки
			foreach (var del in ParcelsDelete)
			{
				Parcels.Add(del);
			}
			ParcelsDelete.Clear();

			//Уведомляем об изменении свойств
			RaisePropertyChanged(nameof(Parcels));
			RaisePropertyChanged(nameof(ParcelsViewSource));
		}

		/// <summary>
		/// Загружает данные из базы данных
		/// </summary>
		async protected Task LoadDataAsync()
		{
			//Загружаем справочники
			ContextDb.UtilisationTypes.Load();
			ContextDb.CertificationDocumentTypes.Load();
			UtilisationTypes = new ObservableCollection<UtilisationType>(ContextDb.UtilisationTypes.Local);
			CertificationDocumentTypes = new ObservableCollection<CertificationDocumentType>(ContextDb.CertificationDocumentTypes.Local);
			
			//Быстро загружаем несколько объектов
			ContextDb.JuridiqueObjets.OfType<Parcelle>()
					 .Include(i => i.Impots)
					 .Include(ut => ut.Utilisations)
					 .Include(s => s.CertificationDocuments)
					 .OrderBy(p => p.NUP)
					 .Take(100)
					 .Load();
			Parcels = new ObservableCollection<Parcelle>(ContextDb.JuridiqueObjets.Local.OfType<Parcelle>());
			ParcelsViewSource.Source = Parcels;

			//Уведомляем подписчиков об изменении данных
			RaisePropertyChanged(nameof(UtilisationTypes));
			RaisePropertyChanged(nameof(CertificationDocumentTypes));
			RaisePropertyChanged(nameof(Parcels));
			RaisePropertyChanged(nameof(ParcelsViewSource));

			//Продолжаем загрузку оставшихся данных
			await ContextDb.JuridiqueObjets.OfType<Parcelle>()
							.Include(i => i.Impots)
							.Include(ut => ut.Utilisations)
							.Include(s => s.CertificationDocuments)
							.OrderBy(p => p.NUP)
							.LoadAsync();
			ParcelsViewSource.Source = ContextDb.JuridiqueObjets.Local;
			Parcels = new ObservableCollection<Parcelle>(ContextDb.JuridiqueObjets.Local.OfType<Parcelle>());
			
			//Уведомляем подписчиков об изменении данных
			RaisePropertyChanged(nameof(Parcels));
			RaisePropertyChanged(nameof(ParcelsViewSource));
		}
		/// <summary>
		/// Обновляет данные справочников изменившихся в БД
		/// </summary>
		/// <returns></returns>
		async protected Task RefreshDataAsync()
		{
			//Актуальные справочники
			ObservableCollection<UtilisationType> actualUtilisations;
			ObservableCollection<CertificationDocumentType> actualCertificationDocumentTypes;
			
			//Загружаем актуальные справочники
			using (CadastrBeninDB db = ContextFactory.Create())
			{
				await db.UtilisationTypes.LoadAsync();
				await db.CertificationDocumentTypes.LoadAsync();

				actualUtilisations = new ObservableCollection<UtilisationType>(db.UtilisationTypes.Local);
				actualCertificationDocumentTypes = new ObservableCollection<CertificationDocumentType>(db.CertificationDocumentTypes.Local);
			}

			//Находим изменившиеся справочники
			var oldUtilisations = ContextDb.UtilisationTypes.Local.Except(actualUtilisations, new UtilizationTypeEqualityComparer()).ToList();
			var oldCertificationDocumentTypes = ContextDb.CertificationDocumentTypes.Local.Except(actualCertificationDocumentTypes, new CertificationDocumentTypeEqualityComparer()).ToList();
			
			//Находим зу, затронутые изменениями
			var oldParcels = Parcels.Where(p => p.Utilisations.Select(u=>u.UtilisationTypeNom)
												.Intersect(
												  oldUtilisations.Select(u => u.Nom))
												.Any()
												||
												p.CertificationDocuments.Select(c=>c.CertificationDocumentTypeNom)
												.Intersect(
												  oldCertificationDocumentTypes.Select(c=>c.Nom))
												.Any()
												).ToList();

			

				ContextDb.UtilisationTypes.Load();
				UtilisationTypes = new ObservableCollection<UtilisationType>(ContextDb.UtilisationTypes.Local);
				//Убираем из справочника устаревшие значения
				foreach (var old in oldUtilisations.ToList())
				{
					UtilisationTypes.Remove(UtilisationTypes.Where(d => d.Nom == old.Nom).Single());
				}

				ContextDb.CertificationDocumentTypes.Load();
				CertificationDocumentTypes = new ObservableCollection<CertificationDocumentType>(ContextDb.CertificationDocumentTypes.Local);
				//Убираем из справочника устаревшие значения
				foreach (var old in oldCertificationDocumentTypes.ToList())
				{
					CertificationDocumentTypes.Remove(CertificationDocumentTypes.Where(d=>d.Nom == old.Nom).Single());
				}


			//Обновляем участки, затронутые изменениями
			foreach (var parcel in oldParcels)
			{
				foreach (var util in parcel.Utilisations)
				{
					if (oldUtilisations.Contains(util.UtilisationType))
						ContextDb.Entry(util).Reload();
				}

				foreach (var doc in parcel.CertificationDocuments)
				{
					if (oldCertificationDocumentTypes.Contains(doc.CertificationDocumentType))
						ContextDb.Entry(doc).Reload();
				}
			}
			
			//Направляем уведомление об изменении данных
			RaisePropertyChanged(nameof(CertificationDocumentTypes));
			RaisePropertyChanged(nameof(UtilisationTypes));
			RaisePropertyChanged(nameof(Parcels));
		}
		/// <summary>
		/// Удаляет записи из подчиненных таблиц у которых datagrid удалил только связь
		/// </summary>
		protected void DeleteDetailRecordTogetherRelationship()
		{
			//Удаляем не связанные с Master-table разрешенные использования
			var delUtil = ContextDb.ChangeTracker.Entries<Utilisation>()
									.Where(u => u.State == EntityState.Modified)
									.Where(u=> u.Entity.JuridiqueObjet == null)
									.ToList();
			foreach (var del in delUtil)
			{
				del.State = EntityState.Deleted;
			}
			
			//Удаляем не связанные с Master-table правоудостоверяющие документы
			var delDoc = ContextDb.ChangeTracker.Entries<CertificationDocument>()
									.Where(u => u.State == EntityState.Modified)
									.Where(u => u.Entity.Parcelle == null)
									.ToList();
			foreach (var del in delDoc)
			{
				del.State = EntityState.Deleted;
			}

			//Удаляем не связанные с Master-table налоги
			var delimpot = ContextDb.ChangeTracker.Entries<Impot>()
									.Where(u => u.State == EntityState.Modified)
									.Where(u => u.Entity.Parcelle == null)
									.ToList();
			foreach (var del in delimpot)
			{
				del.State = EntityState.Deleted;
			}

		}
		/// <summary>
		/// Создает новый земельный участок, но не присоединяет его к базе данных
		/// </summary>
		/// <returns>Земельный участок</returns>
		public Parcelle CreateNewParcel()
		{
			Parcelle newParcel = new Parcelle() {GUID = Guid.NewGuid()};
			return newParcel;
		}
		/// <summary>
		/// Добавляет новый земельный участок к БД, присваивая ему новый кадастровый номер
		/// </summary>
		/// <param name="newParcel"></param>
		public void AddNewParcelInDb(Parcelle newParcel)
		{
			using (var context = ContextFactory.Create())
			{
				context.CadastraleSecteurs_v.Load();
				var cadSect = context.CadastraleSecteurs_v.Local;
				newParcel.CadSecteurNumero = cadSect.Where(s => s.Shape.Intersects(newParcel.Shape)).First().Numero;
				
				newParcel.ParcelleSurCadSecteurNumero = (Parcels.Where(p => p.CadSecteurNumero == newParcel.CadSecteurNumero)
																.Max(p => p.ParcelleSurCadSecteurNumero) ?? 0) + 1;
				newParcel.NUP = string.Format("{0}-{1:D4}", newParcel.CadSecteurNumero, newParcel.ParcelleSurCadSecteurNumero);
				ContextDb.JuridiqueObjets.Attach(newParcel);
				ContextDb.Entry(newParcel).State = EntityState.Added;

				Parcels.Add(newParcel);


				//Уведомляем об изменении свойств
				RaisePropertyChanged(nameof(ParcelsViewSource));
				RaisePropertyChanged(nameof(Parcels));
			}
		}
		/// <summary>
		/// Помечает участок к удалению из базы данных и удаляет его из списка<see cref="Parcels"/>
		/// </summary>
		/// <param name="parcel">Удаляемые земельный участок</param>
		/// <remarks>Удаленный участок сохраняется в кэше <see cref="ParcelsDelete"/></remarks>
		public void DeleteParcelle(Parcelle parcel)
		{
			if (Parcels.Contains(parcel))
			{
				//Сохраняем сведения об удаленном участке для поддержки отмены операции
				if(ContextDb.Entry(parcel).State != EntityState.Added)
					ParcelsDelete.Add(parcel);

				ContextDb.JuridiqueObjets.Remove(parcel);
				Parcels.Remove(parcel);

				//Уведомляем об изменении свойств
				RaisePropertyChanged(nameof(ParcelsViewSource));
				RaisePropertyChanged(nameof(Parcels));
			}
		}

		#region Публичные поля для привязки данных
		/// <summary>
		/// Земельные участки
		/// </summary>
		public ObservableCollection<Parcelle> Parcels { get; set; }
		/// <summary>
		/// Справочник видов разрешенного использования
		/// </summary>
		public ObservableCollection<UtilisationType> UtilisationTypes { get; set; }
		/// <summary>
		/// Справочник типов правоодостоверяющих документов
		/// </summary>
		public ObservableCollection<CertificationDocumentType> CertificationDocumentTypes { get; set; }
		//CollectionViewSource для земельных участков
		public CollectionViewSource ParcelsViewSource { get; set; } = new CollectionViewSource();

		#endregion
		#region Команды
		public bool SaveUndoCommandCanExecute { get { return ContextDb.ChangeTracker.HasChanges(); }}
		
		#endregion

	}
}
