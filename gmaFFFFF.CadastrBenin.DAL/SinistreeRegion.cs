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
	public partial class SinistreeRegion
	{
		public int Id { get; set; }
		public string SinistreeTypeNom { get; set; }
		public string Influence { get; set; }
		public System.Data.Entity.Spatial.DbGeometry Shape { get; set; }
		public Nullable<double> InfluenceCoefficient { get; set; }
	
		public virtual SinistreeType SinistreeType { get; set; }
	}
}
