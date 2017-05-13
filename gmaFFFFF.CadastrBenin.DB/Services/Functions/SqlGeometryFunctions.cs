using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace gmaFFFFF.SQLServer.Geometry
{
	/// <summary>
	/// Класс выполняет различные преобразования над геометрией
	/// </summary>
	public partial class SqlGeometryFunctions
	{
		/// <summary>
		/// Осуществляет смещение геометрии.
		/// Автор: Behnam, https://gis.stackexchange.com/questions/151280/move-shift-a-line-toward-x-and-y-axises-in-sql-server 
		/// </summary>
		/// <param name="g">Геометрия, которую необходимо сместить</param>
		/// <param name="xShift">Смещение по оси х</param>
		/// <param name="yShift">Смещение по оси у</param>
		/// <returns>Смещенная геометрия</returns>
		[Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = false, Name = "STShift", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlGeometry STShift(SqlGeometry g, double xShift, double yShift)
		{
			var sqlGeometryBuilder = new SqlGeometryBuilder();
			var shiftGeometrySink = new ShiftGeometrySink(xShift, yShift, sqlGeometryBuilder);
			g.Populate(shiftGeometrySink);
			return sqlGeometryBuilder.ConstructedGeometry;
		}

		public class ShiftGeometrySink : IGeometrySink110
		{
			private readonly IGeometrySink110 _target;
			private readonly double _xShift;
			private readonly double _yShift;

			public ShiftGeometrySink(double xShift, double yShift, IGeometrySink110 target)
			{
				_target = target;
				_xShift = xShift;
				_yShift = yShift;
			}

			public void SetSrid(int srid)
			{
				_target.SetSrid(srid);
			}

			public void BeginGeometry(OpenGisGeometryType type)
			{
				_target.BeginGeometry(type);
			}

			public void BeginFigure(double x, double y, double? z, double? m)
			{
				_target.BeginFigure(x + _xShift, y + _yShift, z, m);
			}

			public void AddLine(double x, double y, double? z, double? m)
			{
				_target.AddLine(x + _xShift, y + _yShift, z, m);
			}

			public void EndFigure()
			{
				_target.EndFigure();
			}

			public void EndGeometry()
			{
				_target.EndGeometry();
			}

			public void AddCircularArc(double x1, double y1, double? z1, double? m1, double x2, double y2, double? z2, double? m2)
			{
				_target.AddCircularArc(x1, y1, z1, m1, x2, y2, z2, m2);
			}
		}
	}
}
