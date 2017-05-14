using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.DAL;

namespace gmaFFFFF.CadastrBenin.ViewModel.Model
{
	public class DBContextFactory
	{
		/// <summary>
		/// Фабрика по созданию DBContext
		/// </summary>
		/// <returns>Контекст для работы с БД</returns>
		public CadastrBeninDB Create()
		{
			CadastrBeninDB context = new CadastrBeninDB();
			context.Database.CommandTimeout = 60;               //Установка такого большого таймаута вызвана необходимостью учитывать 
																//высокую вероятность того, что экземпляр LocalDb еще не запущен

			return context;
		}
	}
}
