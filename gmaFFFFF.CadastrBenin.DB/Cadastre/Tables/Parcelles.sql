CREATE TABLE Cadastre.Parcelles(
	GUID						   uniqueidentifier	DEFAULT NewID() NOT NULL,
	ParcelleSurCadSecteurNumero	AS				  Cadastre.ExtractParcelNumberInCadSector(NUP),
	NUP							varchar(25)		 NOT NULL,
	SiArpentageFrontiere		   bit				 DEFAULT 0 NOT NULL,
	CHECK (Services.RegExIsMatch('^(?<Parcel>(?<Sector>(?<Zone>(?<Department>[0-9]{1,2})-[0-9]{1,2})-[0-9]{1,2})-[0-9]+)$', NUP, null) = 1),
	PRIMARY KEY CLUSTERED (GUID),
	UNIQUE (NUP), 
	FOREIGN KEY (GUID)
	REFERENCES Cadastre.JuridiqueObjets(GUID) ON DELETE CASCADE
)