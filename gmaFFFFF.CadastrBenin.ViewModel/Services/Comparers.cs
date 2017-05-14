using gmaFFFFF.CadastrBenin.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gmaFFFFF.CadastrBenin.ViewModel.Services
{

	public class UtilizationTypeEqualityComparer : IEqualityComparer<UtilisationType>
	{
		public bool Equals(UtilisationType x, UtilisationType y)
		{
			return x.Nom == y.Nom;
		}

		public int GetHashCode(UtilisationType obj)
		{
			return obj.Nom.GetHashCode();
		}
	}

	public class CertificationDocumentTypeEqualityComparer : IEqualityComparer<CertificationDocumentType>
	{
		public bool Equals(CertificationDocumentType x, CertificationDocumentType y)
		{
			return x.Nom == y.Nom;
		}

		public int GetHashCode(CertificationDocumentType obj)
		{
			return obj.Nom.GetHashCode();
		}
	}
}
