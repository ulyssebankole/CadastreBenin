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
	public partial class GeometryEditWindow : Window
	{
		protected GeometryEditViewModel ViewModel;
		public GeometryEditWindow(Parcelle parcel)
		{
			InitializeComponent();
			ViewModel = ((ViewModelLocator) FindResource("ViewModelLocator")).ViewModelEditor;
			ViewModel.NewJob(parcel);
		}

		private void OkButton_OnClick(object sender, RoutedEventArgs e)
		{
			ViewModel.SaveChangesCommand.Execute(null);
			if (ViewModel.IsEndEditing)
			{
				DialogResult = true;
				Close();
			}
		}
	}
}
