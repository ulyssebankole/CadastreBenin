using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace gmaFFFFF.CadastrBenin.DesktopApp
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			string message = e.Exception.Message;
			if (e.Exception.InnerException != null)
				message = message + '\n' + e.Exception.InnerException.Message;

			//Два вызова MessageBox.Show необходимы потому, что из-за splashScreen первый вызов исчезает сразу после появления
			MessageBox.Show(message,"Неразрешимая ситуация");
			MessageBox.Show(message, "Неразрешимая ситуация", MessageBoxButton.OK);

		}
	}
}
