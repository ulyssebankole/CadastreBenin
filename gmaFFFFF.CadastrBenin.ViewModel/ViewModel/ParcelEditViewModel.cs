using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using DotSpatial.Projections;
using DotSpatial.Topology.Utilities;
using gmaFFFFF.CadastrBenin.DAL;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Maps.MapControl.WPF;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	public class ParcelEditViewModel : ViewModelBase
	{
		/// <summary>
		/// Контекст базы данных
		/// </summary>
		private CadastrBeninDB ContextDb;

		public ParcelEditViewModel(DBContextFactory contextFactory)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				ContextDb = contextFactory.Create();
				ContextDb.Database.CommandTimeout = 60;
				LoadDataAsync();
			}

			//Инициализация комманд
			SaveChangesCommand = new RelayCommand(SaveChanges/*,()=> SaveUndoCommandCanExecute*/);
			UndoChangesCommand = new RelayCommand(UndoChanges/*,()=> SaveUndoCommandCanExecute*/);
		}

		public override void Cleanup()
		{
			ContextDb.Dispose();
			base.Cleanup();
		}

		/// <summary>
		/// Сохраняет все внесенные изменения
		/// </summary>
		protected void SaveChanges() { if (SaveUndoCommandCanExecute) ContextDb.SaveChanges(); }
		/// <summary>
		/// Отменяет все внесенные изменения
		/// </summary>
		protected void UndoChanges()
		{
			if (!SaveUndoCommandCanExecute)
				return;
			//Источник https://code.msdn.microsoft.com/How-to-undo-the-changes-in-00aed3c4
			foreach (DbEntityEntry entry in ContextDb.ChangeTracker.Entries())
			{
				switch (entry.State)
				{
					// Under the covers, changing the state of an entity from  
					// Modified to Unchanged first sets the values of all  
					// properties to the original values that were read from  
					// the database when it was queried, and then marks the  
					// entity as Unchanged. This will also reject changes to  
					// FK relationships since the original value of the FK  
					// will be restored. 
					case EntityState.Modified:
						entry.State = EntityState.Unchanged;
						break;
					case EntityState.Added:
						entry.State = EntityState.Detached;
						break;
					// If the EntityState is the Deleted, reload the date from the database.   
					case EntityState.Deleted:
						entry.Reload();
						break;
					default: break;
				}
			}
			RaisePropertyChanged(nameof(Parcels));
		}

		/// <summary>
		/// Загружает данные из базы данных
		/// </summary>
		async protected void LoadDataAsync()
		{
			ContextDb.UtilisationTypes.Load();
			ContextDb.CertificationDocumentTypes.Load();
			UtilisationTypes = new ObservableCollection<UtilisationType>(ContextDb.UtilisationTypes.Local);
			CertificationDocumentTypes = new ObservableCollection<CertificationDocumentType>(ContextDb.CertificationDocumentTypes.Local);
			RaisePropertyChanged(nameof(UtilisationTypes));
			RaisePropertyChanged(nameof(CertificationDocumentTypes));

			//Быстро загружаем несколько объектов
			ContextDb.JuridiqueObjets.OfType<Parcelle>()
					 .Include(i => i.Impots)
					 .Include(ut => ut.Utilisations)
					 .Include(s => s.CertificationDocuments)
					 .OrderBy(p => p.NUP)
					 .Take(50)
					 .Load();
			Parcels = new ObservableCollection<Parcelle>(ContextDb.JuridiqueObjets.Local.OfType<Parcelle>());
			ParcelsViewSource.Source = Parcels;
			RaisePropertyChanged(nameof(Parcels));
			RaisePropertyChanged(nameof(ParcelsViewSource));

			//Продолжаем загрузку
			await ContextDb.JuridiqueObjets.OfType<Parcelle>()
							.Include(i => i.Impots)
							.Include(ut => ut.Utilisations)
							.Include(s => s.CertificationDocuments)
							.OrderBy(p => p.NUP)
							.LoadAsync();
			ParcelsViewSource.Source = ContextDb.JuridiqueObjets.Local;
			Parcels = new ObservableCollection<Parcelle>(ContextDb.JuridiqueObjets.Local.OfType<Parcelle>());
			RaisePropertyChanged(nameof(Parcels));
			RaisePropertyChanged(nameof(ParcelsViewSource));
	
		}


		#region Публичные поля для привязки данных
		public ObservableCollection<Parcelle> Parcels { get; set; }
		public ObservableCollection<UtilisationType> UtilisationTypes { get; set; }
		public ObservableCollection<CertificationDocumentType> CertificationDocumentTypes { get; set; }
		public CollectionViewSource ParcelsViewSource { get; set; } = new CollectionViewSource();

		#endregion
		#region Команды
		public RelayCommand SaveChangesCommand { get; set; }
		public RelayCommand UndoChangesCommand { get; set; }
		public bool SaveUndoCommandCanExecute { get { return ContextDb.ChangeTracker.HasChanges(); }}
		#endregion

	}
}
