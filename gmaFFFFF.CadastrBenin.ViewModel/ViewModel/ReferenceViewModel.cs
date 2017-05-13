﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using gmaFFFFF.CadastrBenin.DAL;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.Command;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	public class ReferenceViewModel : ViewModelBase
	{
		/// <summary>
		/// Контекст базы данных
		/// </summary>
		private CadastrBeninDB ContextDb;
		public ReferenceViewModel(DBContextFactory contextFactory)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				ContextDb = contextFactory.Create();
				LoadDataAsync();
			}
			//Инициализация комманд
			SaveChangesCommand = new RelayCommand(SaveChanges);
			UndoChangesCommand = new RelayCommand(UndoChanges);
		}
		public override void Cleanup()
		{
			ContextDb.Dispose();
			base.Cleanup();
		}

		/// <summary>
		/// Загружает данные из базы данных
		/// </summary>
		async protected void LoadDataAsync()
		{
			//Загружаем справочники
			await ContextDb.CertificationDocumentTypes.LoadAsync();
			await ContextDb.SinistreeTypes.LoadAsync();
			await ContextDb.SujetDocumentTypes.LoadAsync();
			await ContextDb.UtilisationTypes.LoadAsync();
			await ContextDb.JuridiqueTypes.Include(j => j.TransactionTypes).LoadAsync();
			await ContextDb.Etats.LoadAsync();
			await ContextDb.CadastraleDivisions.LoadAsync();

			//Сохраняем справочники в общедоступных полях 
			CertificationDocTypesDB   = ContextDb.CertificationDocumentTypes.Local;
			SinistreeTypesDB		  = ContextDb.SinistreeTypes.Local;
			SujetDocumentTypesDB	  = ContextDb.SujetDocumentTypes.Local;
			UtilisationTypesDB		= ContextDb.UtilisationTypes.Local;
			JuridiqueTypesDB		  = ContextDb.JuridiqueTypes.Local;
			EtatsDB				   = ContextDb.Etats.Local;
			CadastraleDivisions	 = ContextDb.CadastraleDivisions.Local;

			//Обеспечиваем упорядочивание по алфавиту
			((ListCollectionView)CollectionViewSource.GetDefaultView(CertificationDocTypes))
					.SortDescriptions.Add(new SortDescription(nameof(CertificationDocumentType.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(SinistreeTypes))
					.SortDescriptions.Add(new SortDescription(nameof(SinistreeType.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(SujetDocumentTypes))
					.SortDescriptions.Add(new SortDescription(nameof(SujetDocumentType.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(UtilisationTypes))
					.SortDescriptions.Add(new SortDescription(nameof(UtilisationType.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(JuridiqueTypes))
					.SortDescriptions.Add(new SortDescription(nameof(JuridiqueType.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(Etats))
					.SortDescriptions.Add(new SortDescription(nameof(Etat.Nom), ListSortDirection.Ascending));
			((ListCollectionView)CollectionViewSource.GetDefaultView(CadastraleDivisions))
					.SortDescriptions.Add(new SortDescription(nameof(CadastraleDivision.Numero), ListSortDirection.Ascending));

			//Направляем уведомление об изменении свойств
			RaisePropertyChanged(nameof(CertificationDocTypesDB));
			RaisePropertyChanged(nameof(SinistreeTypesDB));
			RaisePropertyChanged(nameof(SujetDocumentTypesDB));
			RaisePropertyChanged(nameof(UtilisationTypesDB));
			RaisePropertyChanged(nameof(JuridiqueTypesDB));
			RaisePropertyChanged(nameof(EtatsDB));
			RaisePropertyChanged(nameof(CadastraleDivisions));

			//Создаем прокси объекты для справочников, т.к. Entity Framework не поддерживает изменение ключей в БД
			CreateProxy();
		}

		/// <summary>
		/// Сохраяет все внесенные изменения
		/// </summary>
		protected void SaveChanges()
		{
			//Кривой код этой функции вызван отсутствием поддержки обновления ключей БД со стороны EntityFramework.
			//Из-за этого операции вставки, удаления и изменения проводить через объект заместитель.

			try
			{
			#region Шаблонный код обработки заместителей
			//CertificationDocTypes
			var delCertificationDocTypes = CertificationDocTypesDB.Except(
											CertificationDocTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delCertificationDocTypes)
			{
				ContextDb.Entry<CertificationDocumentType>(del).State = EntityState.Deleted;
			}

			var addCertificationDocTypes = CertificationDocTypes.Where(d => d.Added);
			foreach (var add in addCertificationDocTypes)
			{
				CertificationDocumentType newType = new CertificationDocumentType() {Nom = add.Nom};
				ContextDb.CertificationDocumentTypes.Attach(newType);
				ContextDb.Entry<CertificationDocumentType>(newType).State = EntityState.Added;
			}

			var modCertificationDocTypes = CertificationDocTypes.Where(m => m.Modifed);
			foreach (var mod in modCertificationDocTypes)
			{
				CertificationDocumentType oldType = mod.InnerReference;
				CertificationDocumentType newType = new CertificationDocumentType() { Nom = mod.Nom};
				ContextDb.CertificationDocumentTypes.Attach(newType);
				ContextDb.Entry<CertificationDocumentType>(newType).State = EntityState.Added;

				foreach (var child in oldType.CertificationDocuments.ToList())
				{
					child.CertificationDocumentTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.CertificationDocumentTypeNom).IsModified = true;
				}
				

				ContextDb.Entry<CertificationDocumentType>(oldType).State = EntityState.Deleted;
			}

			//SinistreeTypes
			var delSinistreeTypes = SinistreeTypesDB.Except(
											SinistreeTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delSinistreeTypes)
			{
				ContextDb.Entry<SinistreeType>(del).State = EntityState.Deleted;
			}

			var addSinistreeTypes = SinistreeTypes.Where(d => d.Added);
			foreach (var add in addSinistreeTypes)
			{
				SinistreeType newType = new SinistreeType() { Nom = add.Nom };
				ContextDb.SinistreeTypes.Attach(newType);
				ContextDb.Entry<SinistreeType>(newType).State = EntityState.Added;
			}

			var modSinistreeTypes = SinistreeTypes.Where(m => m.Modifed);
			foreach (var mod in modSinistreeTypes)
			{
				SinistreeType oldType = mod.InnerReference;
				SinistreeType newType = new SinistreeType() { Nom = mod.Nom };
				ContextDb.SinistreeTypes.Attach(newType);
				ContextDb.Entry<SinistreeType>(newType).State = EntityState.Added;

				foreach (var child in oldType.SinistreeRegions.ToList())
				{
					child.SinistreeTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.SinistreeTypeNom).IsModified = true;
				}


				ContextDb.Entry<SinistreeType>(oldType).State = EntityState.Deleted;
			}

			//SujetDocumentTypes
			var delSujetDocumentTypes = SujetDocumentTypesDB.Except(
											SujetDocumentTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delSujetDocumentTypes)
			{
				ContextDb.Entry<SujetDocumentType>(del).State = EntityState.Deleted;
			}

			var addSujetDocumentTypes = SujetDocumentTypes.Where(d => d.Added);
			foreach (var add in addSujetDocumentTypes)
			{
				SujetDocumentType newType = new SujetDocumentType() { Nom = add.Nom };
				ContextDb.SujetDocumentTypes.Attach(newType);
				ContextDb.Entry<SujetDocumentType>(newType).State = EntityState.Added;
			}

			var modSujetDocumentTypes = SujetDocumentTypes.Where(m => m.Modifed);
			foreach (var mod in modSujetDocumentTypes)
			{
				SujetDocumentType oldType = mod.InnerReference;
				SujetDocumentType newType = new SujetDocumentType() { Nom = mod.Nom };
				ContextDb.SujetDocumentTypes.Attach(newType);
				ContextDb.Entry<SujetDocumentType>(newType).State = EntityState.Added;

				foreach (var child in oldType.SujetDocuments.ToList())
				{
					child.SujetDocumentTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.SujetDocumentTypeNom).IsModified = true;
				}


				ContextDb.Entry<SujetDocumentType>(oldType).State = EntityState.Deleted;
			}


			//UtilisationTypes
			var delUtilisationTypes = UtilisationTypesDB.Except(
											UtilisationTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delUtilisationTypes)
			{
				ContextDb.Entry<UtilisationType>(del).State = EntityState.Deleted;
			}

			var addUtilisationTypes = UtilisationTypes.Where(d => d.Added);
			foreach (var add in addUtilisationTypes)
			{
				UtilisationType newType = new UtilisationType() { Nom = add.Nom };
				ContextDb.UtilisationTypes.Attach(newType);
				ContextDb.Entry<UtilisationType>(newType).State = EntityState.Added;
			}

			var modUtilisationTypes = UtilisationTypes.Where(m => m.Modifed);
			foreach (var mod in modUtilisationTypes)
			{
				UtilisationType oldType = mod.InnerReference;
				UtilisationType newType = new UtilisationType() { Nom = mod.Nom };
				ContextDb.UtilisationTypes.Attach(newType);
				ContextDb.Entry<UtilisationType>(newType).State = EntityState.Added;

				foreach (var child in oldType.Utilisations.ToList())
				{
					child.UtilisationTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.UtilisationTypeNom).IsModified = true;
				}


				ContextDb.Entry<UtilisationType>(oldType).State = EntityState.Deleted;
			}

			//Etats
			var delEtats = EtatsDB.Except(
											Etats.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delEtats)
			{
				ContextDb.Entry<Etat>(del).State = EntityState.Deleted;
			}

			var addEtats = Etats.Where(d => d.Added);
			foreach (var add in addEtats)
			{
				Etat newType = new Etat() { Nom = add.Nom };
				ContextDb.Etats.Attach(newType);
				ContextDb.Entry<Etat>(newType).State = EntityState.Added;
			}

			var modEtats = Etats.Where(m => m.Modifed);
			foreach (var mod in modEtats)
			{
				Etat oldType = mod.InnerReference;
				Etat newType = new Etat() { Nom = mod.Nom };
				ContextDb.Etats.Attach(newType);
				ContextDb.Entry<Etat>(newType).State = EntityState.Added;

				foreach (var child in oldType.Sujets.ToList())
				{
					child.Nationalite = newType.Nom;
					ContextDb.Entry(child).Property(p => p.Nationalite).IsModified = true;
				}


				ContextDb.Entry<Etat>(oldType).State = EntityState.Deleted;
			}

			//JuridiqueTypes
			var delJuridiqueTypes = JuridiqueTypesDB.Except(
											JuridiqueTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delJuridiqueTypes)
			{
				foreach (var tran in del.TransactionTypes.ToList())
				{
					ContextDb.Entry<TransactionType>(tran).State = EntityState.Deleted; 
				}
				ContextDb.Entry<JuridiqueType>(del).State = EntityState.Deleted;
			}

			var addJuridiqueTypes = JuridiqueTypes.Where(d => d.Added);
			foreach (var add in addJuridiqueTypes)
			{
				JuridiqueType newType = new JuridiqueType() { Nom = add.Nom };
				ContextDb.JuridiqueTypes.Attach(newType);
				ContextDb.Entry<JuridiqueType>(newType).State = EntityState.Added;
			}

			var modJuridiqueTypes = JuridiqueTypes.Where(m => m.Modifed);
			foreach (var mod in modJuridiqueTypes)
			{
				JuridiqueType oldType = mod.InnerReference;
				JuridiqueType newType = new JuridiqueType() { Nom = mod.Nom };
				ContextDb.JuridiqueTypes.Attach(newType);
				ContextDb.Entry<JuridiqueType>(newType).State = EntityState.Added;

				foreach (var child in oldType.TransactionTypes.ToList())
				{
					child.JuridiqueTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.JuridiqueTypeNom).IsModified = true;
				}


				ContextDb.Entry<JuridiqueType>(oldType).State = EntityState.Deleted;
			}
			
			ObservableCollection <TransactionType> TransactionTypesDB	 = new ObservableCollection<TransactionType>(JuridiqueTypesDB.SelectMany(j => j.TransactionTypes).ToList());
			foreach (var jur in JuridiqueTypes)
			{
				foreach (var tr in jur.TransactionTypes)
				{
					if (tr.JuridiqueType == null)
						tr.JuridiqueType = jur;
				}
			}
			ObservableCollection<TransactionTypeProxy> TransactionTypes =  new ObservableCollection<TransactionTypeProxy>(JuridiqueTypes.SelectMany(j => j.TransactionTypes).ToList());

			//TransactionTypes
			var delTransactionTypes = TransactionTypesDB.Except(
											TransactionTypes.Where(d => !d.Added).Select(d => d.InnerReference)).ToList();
			foreach (var del in delTransactionTypes)
			{
				ContextDb.Entry<TransactionType>(del).State = EntityState.Deleted;
			}

			var addTransactionTypes = TransactionTypes.Where(d => d.Added);
			foreach (var add in addTransactionTypes)
			{
				TransactionType newType = new TransactionType() { Nom = add.Nom, JuridiqueTypeNom = add.JuridiqueType.Nom};
				ContextDb.TransactionTypes.Attach(newType);
				ContextDb.Entry<TransactionType>(newType).State = EntityState.Added;
			}

			var modTransactionTypes = TransactionTypes.Where(m => m.Modifed);
			foreach (var mod in modTransactionTypes)
			{
				TransactionType oldType = mod.InnerReference;
				TransactionType newType = new TransactionType() { Nom = mod.Nom, JuridiqueTypeNom = mod.JuridiqueType.Nom };
				ContextDb.TransactionTypes.Attach(newType);
				ContextDb.Entry<TransactionType>(newType).State = EntityState.Added;

				foreach (var child in oldType.Transactions.ToList())
				{
					child.TransactionTypeNom = newType.Nom;
					ContextDb.Entry(child).Property(p => p.TransactionTypeNom).IsModified = true;
				}


				ContextDb.Entry<TransactionType>(oldType).State = EntityState.Deleted;
			}
			
				if (ContextDb.ChangeTracker.HasChanges())
					ContextDb.SaveChanges();


				#endregion
			}
			catch (Exception e)
			{
				string message = "Ошибка ввода: " + e.Message;
				if (e.InnerException != null)
					message = message + '\n' + e.InnerException.Message;
				message += "\nДля предотвращения повреждения данных результаты редактирования аннулированы";

				MessageBox.Show(message, "Ошибка ввода данных", MessageBoxButton.OK);

				//Отменяем внесенные изменения
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
			}

			//Воссоздаем заместителей
			CreateProxy();
		}
		/// <summary>
		/// Отменяет все внесенные изменения
		/// </summary>
		protected void UndoChanges()
		{
			//Воссоздаем заместителей
			CreateProxy();
		}



		/// <summary>
		/// Наполняет заместители реальных объектов данными
		/// </summary>
		protected void CreateProxy()
		{
			//CertificationDocTypes
			CertificationDocTypes.Clear();
			foreach (var doc in CertificationDocTypesDB)
			{
				CertificationDocTypes.Add(new CertificationDocumentTypeProxy(doc));
			}

			//SinistreeTypes
			SinistreeTypes.Clear();
			foreach (var doc in SinistreeTypesDB)
			{
				SinistreeTypes.Add(new SinistreeTypeProxy(doc));
			}
			
			//SujetDocumentTypes
			SujetDocumentTypes.Clear();
			foreach (var doc in SujetDocumentTypesDB)
			{
				SujetDocumentTypes.Add(new SujetDocumentTypeProxy(doc));
			}

			//UtilisationTypes
			UtilisationTypes.Clear();
			foreach (var doc in UtilisationTypesDB)
			{
				UtilisationTypes.Add(new UtilisationTypeProxy(doc));
			}

			//JuridiqueTypes
			JuridiqueTypes.Clear();
			foreach (var doc in JuridiqueTypesDB)
			{
				JuridiqueTypes.Add(new JuridiqueTypeProxy(doc));
			}

			//Etats
			Etats.Clear();
			foreach (var doc in EtatsDB)
			{
				Etats.Add(new EtatProxy(doc));
			}

			
			//Направляем уведомление об изменении свойств
			RaisePropertyChanged(nameof(CertificationDocTypes));
			RaisePropertyChanged(nameof(SinistreeTypes));
			RaisePropertyChanged(nameof(SujetDocumentTypes));
			RaisePropertyChanged(nameof(UtilisationTypes));
			RaisePropertyChanged(nameof(JuridiqueTypes));
			RaisePropertyChanged(nameof(Etats));
		}

		#region Поля для привязки данных
		protected ObservableCollection<CertificationDocumentType>  CertificationDocTypesDB	{ get; set; }
		public ObservableCollection<CertificationDocumentTypeProxy> CertificationDocTypes	{ get; set; } = new ObservableCollection<CertificationDocumentTypeProxy>();
		protected ObservableCollection<SinistreeType>			SinistreeTypesDB			{ get; set; }
		public ObservableCollection<SinistreeTypeProxy>			SinistreeTypes				{ get; set; } = new ObservableCollection<SinistreeTypeProxy>();
		protected ObservableCollection<SujetDocumentType>		SujetDocumentTypesDB		{ get; set; }
		public ObservableCollection<SujetDocumentTypeProxy>		SujetDocumentTypes			{ get; set; } = new ObservableCollection<SujetDocumentTypeProxy>();
		protected ObservableCollection<UtilisationType>			UtilisationTypesDB			{ get; set; }
		public ObservableCollection<UtilisationTypeProxy>		UtilisationTypes			{ get; set; } = new ObservableCollection<UtilisationTypeProxy>();
		protected ObservableCollection<JuridiqueType>			JuridiqueTypesDB			{ get; set; }
		public ObservableCollection<JuridiqueTypeProxy>			JuridiqueTypes				{ get; set; } = new ObservableCollection<JuridiqueTypeProxy>();
		protected ObservableCollection<Etat>					EtatsDB						{ get; set; }
		public ObservableCollection<EtatProxy>					Etats						{ get; set; } = new ObservableCollection<EtatProxy>();
		public ObservableCollection<CadastraleDivision>			CadastraleDivisions			{ get; set; }
		#endregion

		#region Команды

		public RelayCommand SaveChangesCommand { get; set; }
		public RelayCommand UndoChangesCommand { get; set; }
		#endregion
	}
}


