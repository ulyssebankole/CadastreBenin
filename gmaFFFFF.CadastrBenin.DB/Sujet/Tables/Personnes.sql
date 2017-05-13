CREATE TABLE Sujet.Personnes(
	GUID			 uniqueidentifier	NOT NULL,
	Nom			  nvarchar(50)		NOT NULL,
	Prenoms		  nvarchar(150)	   NOT NULL,
	Sexe			 bit				 NULL,
	NaissanceLieu	nvarchar(50)		NULL,
	NaissanceDate	date				NULL,
	Profession	   nvarchar(50)		NULL,
	NomEtPrenome	 as Nom + ' ' + Prenoms PERSISTED,
	PRIMARY KEY CLUSTERED (GUID), 
	FOREIGN KEY (GUID)
	REFERENCES Sujet.Sujets(GUID) ON DELETE CASCADE
)