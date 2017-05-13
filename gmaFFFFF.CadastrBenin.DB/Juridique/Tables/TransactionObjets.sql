CREATE TABLE Juridique.TransactionObjets(
	Id					int				 IDENTITY(1,1),
	JuridiqueObjetGUID	uniqueidentifier	DEFAULT NewID() NOT NULL,
	TransactionGUID	   uniqueidentifier	NOT NULL,
	PRIMARY KEY NONCLUSTERED (Id),
	UNIQUE CLUSTERED (JuridiqueObjetGUID, TransactionGUID), 
	FOREIGN KEY (TransactionGUID)
	REFERENCES Juridique.Transactions(GUID) ON DELETE CASCADE,
	FOREIGN KEY (JuridiqueObjetGUID)
	REFERENCES Cadastre.JuridiqueObjets(GUID)
)