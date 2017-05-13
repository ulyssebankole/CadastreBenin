CREATE TABLE Cadastre.Impots(
	Id			  int				 IDENTITY(1,1),
	ParcelleGUID	uniqueidentifier	DEFAULT NewID() NOT NULL,
	Date			date				DEFAULT cast (GetDate() as Date) NOT NULL,
	Somme		   decimal(19, 2)	  NOT NULL,
	Document		nvarchar(50)		NOT NULL,
	PRIMARY KEY NONCLUSTERED (Id),
	UNIQUE CLUSTERED (ParcelleGUID, Date), 
	FOREIGN KEY (ParcelleGUID)
	REFERENCES Cadastre.Parcelles(GUID) ON DELETE CASCADE
)