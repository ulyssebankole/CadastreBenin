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
	public partial class Parcelles_v
	{
		public string NUP { get; set; }
		public bool SiArpentageFrontiere { get; set; }
		public System.Data.Entity.Spatial.DbGeometry Shape { get; set; }
		public Nullable<double> Superficie { get; set; }
	}
}
