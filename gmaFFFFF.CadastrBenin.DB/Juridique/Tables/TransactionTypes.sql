CREATE TABLE Juridique.TransactionTypes(
	Nom				 nvarchar(50)	NOT NULL,
	JuridiqueTypeNom	nvarchar(50)	NOT NULL,
	PRIMARY KEY CLUSTERED (Nom), 
	FOREIGN KEY (JuridiqueTypeNom)
	REFERENCES Juridique.JuridiqueTypes(Nom) ON UPDATE CASCADE
)