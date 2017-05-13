using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.DAL;
using Microsoft.Maps.MapControl.WPF;
using PropertyChanged;

namespace gmaFFFFF.CadastrBenin.ViewModel.Model
{

	[ImplementPropertyChanged]
	public class ParcelModel
	{
		public Parcelle Parcel { set; get; }
		public LocationCollection WgsLocation { set; get; }
		public Location Centroid { set; get; }
		public string LastUtilisation {
			get
			{
				return (from ut in Parcel.Utilisations
					orderby ut.Date descending
					select ut).FirstOrDefault()?.UtilisationTypeNom;
			}
		}
		public decimal? LastImpot {
			get
			{
				return (from ut in Parcel.Impots
					orderby ut.Date descending
					select ut).FirstOrDefault()?.Somme;
			}
		}
		public int? BuildingCount { set; get; }
		
		/// <summary>
		/// Площадь объекта. Причина введения - операция Binding не сробатывает со свойством JuridiqueObjet.Superficie
		/// </summary>
		public int Area
		{
			get { return Parcel.Superficie??0; }
		}
		public ObservableCollection<BuildingOnParcelModel> BuildingsOnParcel { set; get; }
		public ObservableCollection<Sinestree_Parcelles_v> Sinistrees { set; get; }
		public ObservableCollection<JuridiqueSituations_v> Rights { set; get; }
		public double SinistreeInfluence { get
		{
			return Sinistrees.GroupBy(s => s.SinistreeTypeNom)
							 .Aggregate(1.0, (av, v) => av * 
											 (v.Sum(s => s.Area*s.InfluenceCoefficient)/v.Sum(s => s.Area)) ?? 1);
		} }
	}

	public class BuildingOnParcelModel
	{
		public Parcel_ImmeubleIntersect Building { set; get; }
		public LocationCollection WgsLocation { set; get; }
		public Location Centroid { set; get; }		
	}
}
