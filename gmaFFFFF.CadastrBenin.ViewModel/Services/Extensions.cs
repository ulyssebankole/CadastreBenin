using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Data.Entity.Spatial;


namespace gmaFFFFF.CadastrBenin.ViewModel.Services
{
	public static class Extensions
	{
		public static SqlGeometry ToSqlGeometry(this DbGeometry dbGeometry)
		{
			if (dbGeometry == null)
				return null;
			return SqlGeometry.STGeomFromWKB(new SqlBytes(dbGeometry.AsBinary()), dbGeometry.CoordinateSystemId);
		}
		public static DbGeometry ToDbGeometry(this SqlGeometry sqlGeometry)
		{
			if (sqlGeometry == null)
				return null;
			return DbGeometry.FromBinary(sqlGeometry.STAsBinary().Buffer, sqlGeometry.STSrid.Value);
		}
		public static SqlGeography ToSqlGeography(this DbGeography dbGeography)
		{
			if (dbGeography == null)
				return null;
			return SqlGeography.STGeomFromWKB(new SqlBytes(dbGeography.AsBinary()), dbGeography.CoordinateSystemId);
		}
		
		public static DbGeography ToDbGeography(this SqlGeography sqlGeography)
		{
			if (sqlGeography == null)
				return null;
			return DbGeography.FromBinary(sqlGeography.STAsBinary().Buffer, sqlGeography.STSrid.Value);
		}
	}
}
