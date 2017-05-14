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
using System.Windows.Shapes;
using gmaFFFFF.CadastrBenin.DAL;
using gmaFFFFF.CadastrBenin.ViewModel;
using gmaFFFFF.CadastrBenin.ViewModel.Model;

namespace gmaFFFFF.CadastrBenin.DesktopApp
{
	/// <summary>
	/// Логика взаимодействия для EditParcelGeometryWindow.xaml
	/// </summary>
	public partial class EditParcelGeometryWindow : Window
	{
		protected EditParcelGeometryViewModel ViewModel;
		public EditParcelGeometryWindow(Parcelle parcel)
		{
			InitializeComponent();
			ViewModel = ((ViewModelLocator) FindResource("ViewModelLocator")).ParcelGeometryViewModelEditor;
			ViewModel.NewJob(parcel);
		}

		private void Save_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Wkt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			}
			catch (Exception ex)
			{
				string message = String.Format("Ошибка ввод координат: {0}",ex.Message);
				e.Handled = true;
				MessageBox.Show(message);
			}
		}
	}
}
