using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmaFFFFF.CadastrBenin.DAL
{
	public partial class CadastrBeninDB
	{
		public void DiscardAllChanges()
		{
			//Отменяем внесенные изменения
			//Источник https://code.msdn.microsoft.com/How-to-undo-the-changes-in-00aed3c4
			//Поправка для удаленных сущностей: https://stackoverflow.com/questions/16437083/dbcontext-discard-changes-without-disposing
			foreach (DbEntityEntry entry in ChangeTracker.Entries())
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
						entry.State = EntityState.Modified; //Revert changes made to deleted entity.
						entry.State = EntityState.Unchanged;
						//entry.Reload();					//Этот код не перезагружает удаленные данные
						break;
					default: break;
				}
			}
		}
	}
}
