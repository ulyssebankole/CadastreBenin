using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.ViewModel.Services;
using Microsoft.SqlServer.Types;
using System.Data.Entity.Spatial;
using gmaFFFFF.CadastrBenin.DAL;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	public class EditParcelGeometryViewModel : MyViewModelBase
	{
		public EditParcelGeometryViewModel()
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
			}
		}

		protected Parcelle ParcelModel;

		/// <summary>
		/// Настраивает класс для работы
		/// </summary>
		/// <param name="parcel">Границы земельного участка. Если перед null, то инициализируется пустой геометрией</param>
		public void NewJob(Parcelle parcelModel)
		{
			ParcelModel = parcelModel;
			DbGeometry parcel = ParcelModel.Shape;

			if (parcel != null)
			{
				Parcel = parcel.ToSqlGeometry();
				Srid = parcel.CoordinateSystemId;
			}
			else
			{
				Parcel = new SqlGeometry();
				Srid = 32631;
			}
			//Уведомляем подписчиков об изменении данных
			RaisePropertyChanged(nameof(Parcel));
			RaisePropertyChanged(nameof(Srid));
			RaisePropertyChanged(nameof(isValid));
			RaisePropertyChanged(nameof(Area));
		}

		/// <summary>
		/// Редактируемая геометрия
		/// </summary>
		public SqlGeometry Parcel { get; set; }
		/// <summary>
		/// Идентификатор системы координат
		/// </summary>
		public int Srid { get; set; }
		/// <summary>
		/// Площадь
		/// </summary>
		public double Area { get { return Parcel.STArea().Value; } }
		/// <summary>
		/// Возвращает true если геометрия имеет правильный формат, определенный OGC
		/// </summary>
		public bool isValid {get { return Parcel.STIsValid().Value; } }
		
		
	}
}
