using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Microsoft.SqlServer.Types;

namespace gmaFFFFF.CadastrBenin.DesktopApp
{
	/// <summary>
	/// Конвертор, возвращающий true если преобразуемое значение меньше заданного
	/// </summary>
	[ValueConversion(typeof(double), typeof(bool))]
	public class isLessThanConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double comparValue;
			double paramValue;
			double.TryParse(value.ToString(), out comparValue);
			double.TryParse(parameter.ToString(), out paramValue);
			return (comparValue < paramValue ? true : false);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	/// <summary>
	/// Конвертор, возвращающий true если преобразуемое значение больше заданного
	/// </summary>
	[ValueConversion(typeof(double), typeof(bool))]
	public class isMoreThanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double comparValue;
			double paramValue;
			double.TryParse(value.ToString(), out comparValue);
			double.TryParse(parameter.ToString(), out paramValue);
			return (comparValue > paramValue ? true : false);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Класс определяет попадает ли заданный объект в указанные диапазон.
	/// Порядок параметров: сравниваемое значение, минимум, максимум
	/// </summary>
	public class isLessAndMoreConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() == 3)
			{
				double comparValue;
				double min;
				double max;
				double.TryParse(values[0].ToString(), out comparValue);
				double.TryParse(values[1].ToString(), out min);
				double.TryParse(values[2].ToString(), out max);

				if (comparValue > min && comparValue < max)
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Заменяет дату 01.01.01 на null
	/// Решение взято здесь https://stackoverflow.com/questions/22587951/datepicker-databinding-sets-default-value-to-1-1-0001-wpf-c-sharp-for-a-not-null
	/// </summary>
	[ValueConversion(typeof(double), typeof(double?))]
	public class OneYearDateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DateTime date = (DateTime)value;
			if (date.Year != 1)
				return date;
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new DateTime(1900,1,1);
			else
				return value;
		}
	}
	/// <summary>
	/// Конвертирует геометрию в текстовый формат WKT
	/// </summary>
	[ValueConversion(typeof(SqlGeometry), typeof(string))]
	public class SqlGeometryConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "";
			SqlGeometry geom = (SqlGeometry) value;
			return new string(geom.STAsText().Value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;
			if ((string)value == "")
				return new SqlGeometry();
			
			SqlChars wkt = new SqlChars(((string)value).ToCharArray());
			return SqlGeometry.STGeomFromText(wkt, 32631).MakeValid();
		}
	}

}
