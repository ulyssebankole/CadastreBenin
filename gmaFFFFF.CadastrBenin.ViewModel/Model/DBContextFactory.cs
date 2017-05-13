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
		/// Предназначен для представления каждой ViewModel своего DBContext
		/// </summary>
		/// <returns></returns>
		public CadastrBeninDB Create()
		{
			return new CadastrBeninDB();
		}
	}
}
