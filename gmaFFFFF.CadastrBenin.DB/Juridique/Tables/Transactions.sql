CREATE TABLE Juridique.Transactions(
	GUID					 uniqueidentifier	DEFAULT NewSequentialId() NOT NULL,
	MereTransactionGUID	  uniqueidentifier	NULL,
	TransactionTypeNom	   nvarchar(50)		NOT NULL,
	TransactionDocumentId	int				 NOT NULL,
	DebutDate				date				DEFAULT cast (GetDate() as Date) NOT NULL,
	FinDate				  date				NULL,
	Observations			 nvarchar(150)	   NULL,
	CHECK (MereTransactionGUID <> GUID),
	PRIMARY KEY CLUSTERED (GUID), 
	FOREIGN KEY (MereTransactionGUID)
	REFERENCES Juridique.Transactions(GUID),
	FOREIGN KEY (TransactionTypeNom)
	REFERENCES Juridique.TransactionTypes(Nom) ON UPDATE CASCADE,
	FOREIGN KEY (TransactionDocumentId)
	REFERENCES Juridique.TransactionDocuments(Id) ON DELETE CASCADE
)