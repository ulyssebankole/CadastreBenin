CREATE TABLE Cadastre.Immeubles(
	GUID						 uniqueidentifier	DEFAULT NewID() NOT NULL,
	NUP						  varchar(30)		 NOT NULL,
	ParcelleNUP				  AS				  Cadastre.ExtractParcelCadastralNumber(NUP) PERSISTED NOT NULL,
	ImmeubleSurParcelleNumero	AS				  Cadastre.ExtractBuildingNumberInParcel(NUP),
	Nom						  nvarchar(100)	   NULL,
	PiecesNombre				 smallint			NULL,
	EtagesNombre				 tinyint			 NULL,
	CHECK (Services.RegExIsMatch(N'^(?<Building>(?<Parcel>(?<Sector>(?<Zone>(?<Department>[0-9]{1,2})-[0-9]{1,2})-[0-9]{1,2})-[0-9]+)(-[a-z]+))$', NUP, null) = 1),
	PRIMARY KEY CLUSTERED (GUID),
	UNIQUE (NUP), 
	FOREIGN KEY (GUID)
	REFERENCES Cadastre.JuridiqueObjets(GUID) ON DELETE CASCADE
)