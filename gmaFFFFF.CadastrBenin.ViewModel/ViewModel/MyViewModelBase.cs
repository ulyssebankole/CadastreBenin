using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using gmaFFFFF.CadastrBenin.ViewModel.Message;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	/// <summary>
	/// Базовый класс для моделей представлений
	/// </summary>
	public class MyViewModelBase : ViewModelBase
	{
		public MyViewModelBase():base()
		{
			//Регистрация на уведомление о необходимости очистки неиспользуемых ресурсов
			Messenger.Default.Register<CleanUpMessage>(this, msg=> Cleanup());
		}

		/// <summary>
		/// Команда сохранения изменений
		/// </summary>
		public ICommand SaveChangesCommand { get; set; } = ApplicationCommands.Save;
		/// <summary>
		/// Команда отмены изменений
		/// </summary>
		public ICommand UndoChangesCommand { get; set; } = ApplicationCommands.Undo;
	}
}
