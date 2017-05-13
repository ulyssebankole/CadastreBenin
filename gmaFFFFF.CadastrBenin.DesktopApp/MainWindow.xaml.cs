using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using gmaFFFFF.CadastrBenin.ViewModel;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using Microsoft.Maps.MapControl.WPF;

namespace gmaFFFFF.CadastrBenin.DesktopApp
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		#region Карта
		/// <summary>
		/// Переключает режим отображения карты
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">Режим переключения содержиться в свойстве Tag</param>
		private void MapTypeSelected(object sender, RoutedEventArgs e)
		{
			var item = sender as RadioButton;
			var mapType = (string)item.Tag;
			switch (mapType)
			{
				case "aerial":
					MyMap.Mode = new AerialMode(false);
					break;
				case "hybrid":
					MyMap.Mode = new AerialMode(true);
					break;
				case "road":
					MyMap.Mode = new RoadMode();
					break;
			}
		}
		#endregion

		private void FindParcels_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			ParcelModel selectedParcel = (ParcelModel) e.AddedItems[0];
			MyMap.SetView(selectedParcel.Centroid,18);
		}
		private void FindParcelPushpinClick(object sender, RoutedEventArgs e)
		{
			if (((Button)sender).Content is Pushpin)
			{
				Pushpin parcel = (Pushpin)((Button)sender).Content;
				FindParcelDetail.DataContext = parcel.DataContext;
				FindParcelDetail.IsOpen = true;
			}
		}

		private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem tab = (TabItem)((TabControl) sender).SelectedItem;
			if (tab != null)
				ApplicationToolbar.DataContext = tab.DataContext;
			
		}
	}
}
