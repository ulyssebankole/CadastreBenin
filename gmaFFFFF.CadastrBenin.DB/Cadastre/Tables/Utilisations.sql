CREATE TABLE Cadastre.Utilisations(
	Id					int				 IDENTITY(1,1),
	UtilisationTypeNom	nvarchar(150)	   NOT NULL,
	JuridiqueObjetGUID	uniqueidentifier	DEFAULT NewID() NOT NULL,
	Date				  date				DEFAULT cast (GetDate() as Date) NOT NULL,
	Document			  nvarchar(50)		NOT NULL,
	PRIMARY KEY NONCLUSTERED (Id),
	UNIQUE CLUSTERED (JuridiqueObjetGUID, Date), 
	FOREIGN KEY (JuridiqueObjetGUID)
	REFERENCES Cadastre.JuridiqueObjets(GUID) ON DELETE CASCADE,
	FOREIGN KEY (UtilisationTypeNom)
	REFERENCES Cadastre.UtilisationTypes(Nom) ON UPDATE CASCADE
)