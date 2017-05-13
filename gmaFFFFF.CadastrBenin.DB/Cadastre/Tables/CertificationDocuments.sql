CREATE TABLE Cadastre.CertificationDocuments(
	Id							  int				 IDENTITY(1,1),
	ParcelleGUID					uniqueidentifier	DEFAULT NewID() NOT NULL,
	CertificationDocumentTypeNom	nvarchar(50)		NOT NULL,
	Date							date				NOT NULL,
	Num							 varchar(10)		 NULL,
	PRIMARY KEY NONCLUSTERED (Id),
	UNIQUE CLUSTERED (ParcelleGUID, Date), 
	FOREIGN KEY (ParcelleGUID)
	REFERENCES Cadastre.Parcelles(GUID) ON DELETE CASCADE,
	FOREIGN KEY (CertificationDocumentTypeNom)
	REFERENCES Cadastre.CertificationDocumentTypes(Nom)
)