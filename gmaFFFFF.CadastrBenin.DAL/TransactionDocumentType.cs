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
	public partial class TransactionDocumentType
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TransactionDocumentType()
		{
			this.TransactionDocuments = new ObservableCollection<TransactionDocument>();
		}
	
		public string Nom { get; set; }
	
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ObservableCollection<TransactionDocument> TransactionDocuments { get; set; }
	}
}
