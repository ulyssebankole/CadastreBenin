using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmaFFFFF.CadastrBenin.DAL
{
	public class DataAccessServices: IDisposable
	{
		#region Набор CollectionView для базы данных
		public CollectionViewSource ParcelleViewSource { get; set; }
		public CollectionViewSource TransactionTypeViewSource { get; set; }
		#endregion
		
		DataAccessServices()
		{
			LoadCollectionViewSources();
		}

		/// <summary>
		/// Сохраяет все внесенные изменения
		/// </summary>
		public void SaveChanges() {contextDb.SaveChanges();}
		/// <summary>
		/// Отменяет все внесенные изменения
		/// </summary>
		public void UndoChanges()
		{
			//Источник https://code.msdn.microsoft.com/How-to-undo-the-changes-in-00aed3c4
			foreach (DbEntityEntry entry in contextDb.ChangeTracker.Entries())
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
		
		#region Очистка памяти
		public void Dispose()
		{
			Dispose(true);
			contextDb.Dispose();
		}
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				//Вызвать метод Dispose на других объектах, которыми владеет данный экземпляр 
				contextDb.Dispose();
			}
			//Освободить неуправляемые ресурсы, которыми владеет только этот экземпляр
		}
		~DataAccessServices()
		{
			Dispose(false);
		}
		#endregion

		private CadastrBeninDB contextDb = new CadastrBeninDB();

		protected void LoadCollectionViewSources()
		{
			//Загружаем справочники
			contextDb.TransactionTypes.Load();
			TransactionTypeViewSource.Source = contextDb.TransactionTypes.Local;

			//Загружаем основные данные
			contextDb.JuridiqueObjets.OfType<Parcelle>().Include(p=>p.Utilisations).Load();
			ParcelleViewSource.Source = contextDb.JuridiqueObjets.Local.OfType<Parcelle>();
		}

	}
}
