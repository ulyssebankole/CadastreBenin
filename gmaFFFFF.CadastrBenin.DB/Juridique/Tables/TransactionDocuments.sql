CREATE TABLE Juridique.TransactionDocuments(
	Id							int			 IDENTITY(1,1),
	TransactionDocumentTypeNom	nvarchar(50)	NOT NULL,
	Date						  date			NULL,
	Num						   varchar(10)	 NULL,
	PRIMARY KEY CLUSTERED (Id), 
	FOREIGN KEY (TransactionDocumentTypeNom)
	REFERENCES Juridique.TransactionDocumentTypes(Nom) ON UPDATE CASCADE
)