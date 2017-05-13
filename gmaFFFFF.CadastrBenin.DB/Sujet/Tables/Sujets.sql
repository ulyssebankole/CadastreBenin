CREATE TABLE Sujet.Sujets(
	GUID			uniqueidentifier	DEFAULT NewSequentialId() NOT NULL,
	SujetTypeNom	nvarchar(50)		NOT NULL,
	Nationalite	 nvarchar(100)	   DEFAULT N'Bénin' NOT NULL,
	Telephone	   varchar(30)		 NULL,
	Adresse		 nvarchar(200)	   NULL,
	BP			  nvarchar(200)	   NULL,
	Email		   nvarchar(50)		NULL,
	PRIMARY KEY CLUSTERED (GUID), 
	FOREIGN KEY (SujetTypeNom)
	REFERENCES Sujet.SujetTypes(Nom) ON UPDATE CASCADE,
	FOREIGN KEY (Nationalite)
	REFERENCES Sujet.Etats(Nom) ON UPDATE CASCADE
)