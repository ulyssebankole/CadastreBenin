using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.ViewModel.Services;
using Microsoft.SqlServer.Types;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using System.Windows.Input;
using gmaFFFFF.CadastrBenin.DAL;
using gmaFFFFF.CadastrBenin.ViewModel.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace gmaFFFFF.CadastrBenin.ViewModel
{
	/// <summary>
	/// Модель представления редактора геометрии
	/// </summary>
	/// <remarks>Чтобы определить новую геометрию - передайте ParcelModel с пустой геометрией </remarks>
	public class GeometryEditViewModel : MyViewModelBase, IDataErrorInfo
	{
		/// <summary>
		/// Земельный участок у которого редактируется геометрия
		/// </summary>
		protected JuridiqueObjet FeatureObjet;
		/// <summary>
		/// Образец геометрии
		/// </summary>
		protected string exampleShape = "POLYGON ((0 0, 1 1, 1 0, 0 0))";
		/// <summary>
		/// Фабрика для создания контекстов данных
		/// </summary>
		private DBContextFactory ContextFactory;
		/// <summary>
		/// Определяет передана ли пустая геометрия
		/// </summary>
		protected bool IsEmptyShape = true;

		
		public GeometryEditViewModel(DBContextFactory contextFactory)
		{
			if (IsInDesignMode)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				ContextFactory = contextFactory;
			}

			SaveChangesCommand = new RelayCommand(SaveChanges);
			RestoreShapeCommand = new RelayCommand(RestoreShape);
		}
		
		/// <summary>
		/// Настраивает класс для работы
		/// </summary>
		/// <param name="parcel">Границы земельного участка. Если перед null, то инициализируется пустой геометрией</param>
		public void NewJob(JuridiqueObjet parcelModel)
		{
			IsEndEditing = false;
			FeatureObjet = parcelModel;
			if (FeatureObjet.Shape == null)
				IsEmptyShape = true;
			RestoreShape();
			
		}
		/// <summary>
		/// Сохраняет изменения геометрии
		/// </summary>
		protected void SaveChanges()
		{
			if (this[nameof(EditShape)] != null)
				return;

			//Конвертация строки в SqlGeometry
			SqlGeometry shape;
			SqlChars wkt = new SqlChars(EditShape.ToCharArray());
			shape = SqlGeometry.STGeomFromText(wkt, 32631).MakeValid();

			FeatureObjet.Shape = shape.ToDbGeometry();
			IsEndEditing = true;
		}
		/// <summary>
		/// Восстанавливает поле с редактируемоей геометрией <see cref="EditShape"/> к исходному состоянию.
		/// </summary>
		protected void RestoreShape()
		{
			if (FeatureObjet.Shape == null)
			{
				EditShape = exampleShape;
			}
			else
			{
				EditShape = FeatureObjet.Shape.AsText();
			}
			//Уведомляем подписчиков об изменении данных
			RaisePropertyChanged(nameof(EditShape));
		}


		/// <summary>
		/// Редактируемая геометрия
		/// </summary>
		public string EditShape { get; set; }
		/// <summary>
		/// Комманда отменяющая результаты текущего сеанса редактирования
		/// </summary>
		public ICommand RestoreShapeCommand { get; set;}
		/// <summary>
		/// Завершена ли операция редактирования
		/// </summary>
		public bool IsEndEditing { get; set; } = false;
		/// <summary>
		/// Индикатор ошибок по имени поля
		/// </summary>
		/// <param name="columnName">Наименование проверяемого свойства</param>
		/// <returns>null - при осутствии ошибок, в противном случае описание ошибки</returns>
		public string this[string columnName]
		{
			get
			{
				if (columnName == nameof(EditShape))
				{
					//Защита от дурака
					if (EditShape == "" || EditShape == null)
						return "Геометрия не может быть пустой";

					SqlGeometry shape;
					
					//Проверка на возможность конвертации в геометрию
					try
					{
						//Конвертация строки в SqlGeometry
						SqlChars wkt = new SqlChars(EditShape.ToCharArray());
						shape = SqlGeometry.STGeomFromText(wkt, 32631).MakeValid();
					}
					catch (Exception ex)
					{
						return String.Format("Ошибка геометрии: {0}", ex.Message);
					}

					//Расположен ли полигон в пределах границ Республики Бенин
					DbGeometry newDbGeometry = shape.ToDbGeometry();
					using (var context = ContextFactory.Create())
					{
						context.CadastraleSecteurs_v.Load();
						var cadSect = context.CadastraleSecteurs_v.Local;
						bool IsBeninWithin = cadSect.Where(s => s.Shape.Intersects(newDbGeometry)).Any();
						if (!IsBeninWithin)
							return "Границы расположены за пределами государственной границы Республики Бенин";
					}

					//Ошибок не найдено
					return null;
				}
				else
					return null;
			}
		}
		/// <summary>
		/// Часть реализации интерфейса IDataErrorInfo. Так как не используется в WPF, то осталось нереализованным
		/// </summary>
		public string Error {get {return null; }}
	}
}
