using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using gmaFFFFF.CadastrBenin.ViewModel;

namespace gmaFFFFF.CadastrBenin.DesktopApp
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Перехватывает все необработанные приложением исключения
		/// </summary>
		private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			string message = e.Exception.Message;
			Exception ie = e.Exception.InnerException;
			while (ie != null)
			{
				message += '\n' + ie.Message;
				ie = ie.InnerException;
			}

			//Два(2) вызова MessageBox.Show необходимы потому, что из-за splashScreen первый вызов исчезает сразу после появления
			MessageBox.Show(message,"Неразрешимая ситуация");
			MessageBox.Show(message, "Неразрешимая ситуация", MessageBoxButton.OK);

		}

		/// <summary>
		/// Завершение приложения
		/// </summary>
		private void App_OnExit(object sender, ExitEventArgs e)
		{
			//Очищаем все ViewModel
			ViewModelLocator.Cleanup();
		}
	}
}
