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
	/// <summary>
	/// Обеспечивает представление полной информации о земельном участке в удобном для представлений виде,
	/// дополняя сущность БД Parcelle дополнительными свойствами
	/// </summary>
	[ImplementPropertyChanged]
	public class ParcelModel
	{
		/// <summary>
		/// Сущность Parcelle из БД
		/// </summary>
		public Parcelle Parcel { set; get; }
		/// <summary>
		/// Границы земельного участка в СК WGS1984
		/// </summary>
		/// <remarks>
		/// Внимание! многоконтурные земельные участки не поддерживаются</remarks>
		public LocationCollection WgsLocation { set; get; }
		/// <summary>
		/// Центроид (центральная точка) земельного участка в СК WGS1984
		/// </summary>
		/// <remarks>
		/// Может использоваться для привязки подписей, кликов и т.д.</remarks>
		public Location Centroid { set; get; }
		/// <summary>
		/// Актуальное (последнее) разрешенное использование зу
		/// </summary>
		public string LastUtilisation {
			get
			{
				return (from ut in Parcel.Utilisations
					orderby ut.Date descending
					select ut).FirstOrDefault()?.UtilisationTypeNom;
			}
		}
		/// <summary>
		/// Актуальная (последняя) налоговая ставка
		/// </summary>
		public decimal? LastImpot {
			get
			{
				return (from ut in Parcel.Impots
					orderby ut.Date descending
					select ut).FirstOrDefault()?.Somme;
			}
		}
		/// <summary>
		/// Количество зданий, которые пересекают границы земельного участка
		/// </summary>
		public int? BuildingCount { set; get; }
		/// <summary>
		/// Площадь объекта. Причина введения - операция Binding не сробатывает со свойством JuridiqueObjet.Superficie
		/// </summary>
		public int Area
		{
			get { return Parcel.Superficie??0; }
		}
		/// <summary>
		/// Здания на земельном участке
		/// </summary>
		public ObservableCollection<BuildingOnParcelModel> BuildingsOnParcel { set; get; }
		/// <summary>
		/// Негативные являения оказывающие влияние на земельный участок
		/// </summary>
		public ObservableCollection<Sinestree_Parcelles_v> Sinistrees { set; get; }
		/// <summary>
		/// Сводная правовая информацию по земельному участку
		/// </summary>
		public ObservableCollection<JuridiqueSituations_v> Rights { set; get; }
		/// <summary>
		/// Расчетный коэффициент суммарного влияния различных негативных явлений на земельный участок
		/// </summary>
		/// <remarks>Коэффициент соответствует произведению коэффициентов всех учитываемых явлений.
		/// Если на одном участке одно явление имеет различную степень влияния, то итоговое влияение данного явления
		/// рассчитывается как средневзвешенное по площади степенени влияния</remarks>
		public double SinistreeInfluence { get
		{
			return Sinistrees.GroupBy(s => s.SinistreeTypeNom)
							 .Aggregate(1.0, (av, v) => av * 
											 (v.Sum(s => s.Area*s.InfluenceCoefficient)/v.Sum(s => s.Area)) ?? 1);
		} }
	}

	/// <summary>
	/// Обеспечивает представление расширенной информации о здании в удобном для представлений виде,
	/// дополняя сущность БД Parcel_ImmeubleIntersect дополнительными свойствами
	/// </summary>
	public class BuildingOnParcelModel
	{
		/// <summary>
		/// Сущность Parcel_ImmeubleIntersect из БД
		/// </summary>
		public Parcel_ImmeubleIntersect Building { set; get; }
		/// <summary>
		/// Границы здания в СК WGS1984
		/// </summary>
		/// <remarks>
		/// Внимание! многоконтурные здания не поддерживаются</remarks>
		public LocationCollection WgsLocation { set; get; }
		/// <summary>
		/// Центроид (центральная точка) земельного участка в СК WGS1984
		/// </summary>
		/// <remarks>
		/// Может использоваться для привязки подписей, кликов и т.д.</remarks>
		public Location Centroid { set; get; }		
	}
}
