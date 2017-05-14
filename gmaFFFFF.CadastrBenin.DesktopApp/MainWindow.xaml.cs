using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using Microsoft.Maps.MapControl.WPF;
using Hardcodet.Wpf.Util;

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
		#region Функциональность карты
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
		/// <summary>
		/// Приближает к выбранном в списке результатов поиска земельному участку
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FindParcels_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;
			ParcelModel selectedParcel = (ParcelModel)e.AddedItems[0];
			MyMap.SetView(selectedParcel.Centroid, 18);
		}
		/// <summary>
		/// Настраивает всплывающее окно с информацие о земельном участке после щелчка на ПушПине
		/// </summary>
		private void FindParcelPushpinClick(object sender, RoutedEventArgs e)
		{
			if (((Button)sender).Content is Pushpin)
			{
				Pushpin parcel = (Pushpin)((Button)sender).Content;
				FindParcelDetail.DataContext = parcel.DataContext;
				FindParcelDetail.IsOpen = true;
			}
		}
		#endregion

		#region Общая функциональность
		/// <summary>
		/// Настраивает контекст ApplicationToolbar для комманд соответствующим выбранной вкладке
		/// </summary>
		private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem tab = (TabItem)((TabControl)sender).SelectedItem;
			if (tab != null)
				ApplicationToolbar.DataContext = tab.DataContext;

		}
		/// <summary>
		/// Действия для подготовки сохранения
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			//Завершить редактирование таблиц
			EditTableEnding();
		}
		/// <summary>
		/// Завершает редактирование таблиц с данными. Если пользователь этого не сделал сам
		/// </summary>
		/// <remarks>Нужно использовать чтобы при сохранении данных был учтен последний ввод</remarks>
		private void EditTableEnding()
		{
			foreach (DataGrid tab in ((TabItem)MainTabControl.SelectedItem).FindChildren<DataGrid>())
			{
				tab.CommitEdit(DataGridEditingUnit.Row, true);
			}
		}
		#endregion



		
	}
}
