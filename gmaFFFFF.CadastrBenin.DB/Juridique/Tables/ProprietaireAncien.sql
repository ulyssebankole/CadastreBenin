CREATE TABLE Juridique.ProprietaireAncien(
	Id				 int				 IDENTITY(1,1),
	TransactionGUID	uniqueidentifier	NOT NULL,
	SujetGUID		  uniqueidentifier	NOT NULL,
	PRIMARY KEY NONCLUSTERED (Id),
	UNIQUE CLUSTERED (TransactionGUID, SujetGUID), 
	FOREIGN KEY (TransactionGUID)
	REFERENCES Juridique.Transactions(GUID) ON DELETE CASCADE,
	FOREIGN KEY (SujetGUID)
	REFERENCES Sujet.Sujets(GUID)
)