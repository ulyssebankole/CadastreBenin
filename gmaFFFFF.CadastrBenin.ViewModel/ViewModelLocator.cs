using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.DAL;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	/// <summary>
	/// Этот класс содержит статические ссылки на все все модели представления 
	/// в приложении и обеспечивает начальную точку для привязки
	/// </summary>
	public class ViewModelLocator
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса ViewModelLocator.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			////if (ViewModelBase.IsInDesignModeStatic)
			////{
			////	// Создайте view services and models времени проектирования
			////	SimpleIoc.Default.Register<IDataService, DesignDataService>();
			////}
			////else
			////{
			////	// Создайте view services and models времени выплнения
			////	SimpleIoc.Default.Register<IDataService, DataService>();
			////}
			
			SimpleIoc.Default.Register<ReferenceViewModel>();
			SimpleIoc.Default.Register<MapViewModel>();
			SimpleIoc.Default.Register<DBContextFactory>();
			SimpleIoc.Default.Register<ParcelEditViewModel>();
		}


		public ReferenceViewModel Reference { get { return ServiceLocator.Current.GetInstance<ReferenceViewModel>(); } }
		public MapViewModel Map { get { return ServiceLocator.Current.GetInstance<MapViewModel>(); } }
		public ParcelEditViewModel ParcelEditor { get { return ServiceLocator.Current.GetInstance<ParcelEditViewModel>(); } }

		public static void Cleanup()
		{
		}
	}
}
