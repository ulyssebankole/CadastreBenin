//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gmaFFFFF.CadastrBenin.DAL
{
	using System;
	using System.Collections.ObjectModel;
	using PropertyChanged;
	
	[ImplementPropertyChanged]
	public partial class ProprietaireAncien
	{
		public int Id { get; set; }
		public System.Guid TransactionGUID { get; set; }
		public System.Guid SujetGUID { get; set; }
	
		public virtual Sujet Sujet { get; set; }
		public virtual Transaction Transaction { get; set; }
	}
}
