using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmaFFFFF.CadastrBenin.DAL;
using PropertyChanged;

namespace gmaFFFFF.CadastrBenin.ViewModel.Model
{
	/// <summary>
	/// Данный класс заглушка для CertificationDocumentType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class CertificationDocumentTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public CertificationDocumentType InnerReference;
		public CertificationDocumentTypeProxy() {}
		public CertificationDocumentTypeProxy(CertificationDocumentType innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed { get {
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;} }
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для SinistreeType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class SinistreeTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public SinistreeType InnerReference;
		public SinistreeTypeProxy() { }
		public SinistreeTypeProxy(SinistreeType innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для SujetDocumentType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class SujetDocumentTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public SujetDocumentType InnerReference;
		public SujetDocumentTypeProxy() { }
		public SujetDocumentTypeProxy(SujetDocumentType innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для UtilisationType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class UtilisationTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public UtilisationType InnerReference;
		public UtilisationTypeProxy() { }
		public UtilisationTypeProxy(UtilisationType innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для Etat нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class EtatProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public Etat InnerReference;
		public EtatProxy() { }
		public EtatProxy(Etat innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для JuridiqueType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class JuridiqueTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public JuridiqueType InnerReference;
		public JuridiqueTypeProxy() { }
		public JuridiqueTypeProxy(JuridiqueType innerReference)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
			TransactionTypes = new ObservableCollection<TransactionTypeProxy>(InnerReference.TransactionTypes.Select(t=>new TransactionTypeProxy(t, this)));
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }

		public virtual ObservableCollection<TransactionTypeProxy> TransactionTypes { get; set; } = new ObservableCollection<TransactionTypeProxy>();
		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}

	/// <summary>
	/// Данный класс заглушка для TransactionType нужен в связи с тем, что Entity Framework не умеет обновлять ключевые поля.
	/// </summary>
	[ImplementPropertyChanged]
	public class TransactionTypeProxy
	{
		/// <summary>
		/// Внутренний объект-справочник для которого создается заглушка
		/// </summary>
		public TransactionType InnerReference;
		
		public TransactionTypeProxy() { }
		public TransactionTypeProxy(TransactionType innerReference, JuridiqueTypeProxy juridiqueType)
		{
			InnerReference = innerReference;
			Nom = innerReference.Nom;
			JuridiqueType = juridiqueType;
		}
		/// <summary>
		/// Ключевое поле справочника
		/// </summary>
		public string Nom { get; set; }

		public virtual JuridiqueTypeProxy JuridiqueType { get; set; }

		/// <summary>
		/// Было ли изменено ключевое поле
		/// </summary>
		public bool Modifed
		{
			get
			{
				if (Added)
					return false;
				else
					return InnerReference.Nom != Nom;
			}
		}
		/// <summary>
		/// Было ли добавлено новое справочное значение
		/// </summary>
		public bool Added { get { return InnerReference == null; } }
	}
}
