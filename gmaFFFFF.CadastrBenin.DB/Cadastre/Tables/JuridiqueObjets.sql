CREATE TABLE Cadastre.JuridiqueObjets(
	GUID					 uniqueidentifier	DEFAULT NewSequentialId() NOT NULL,
	JuridiqueObjetTypeNom	nvarchar(50)		NOT NULL,
	CadSecteurNumero		 varchar(8)		  NOT NULL,
	Shape					geometry			NULL,
	Superficie			   AS				  Round(Shape.STArea(),0) PERSISTED,
	PRIMARY KEY CLUSTERED (GUID),
	UNIQUE (JuridiqueObjetTypeNom, CadSecteurNumero, GUID), 
	FOREIGN KEY (JuridiqueObjetTypeNom)
	REFERENCES Cadastre.JuridiqueObjetTypes(Nom) ON UPDATE CASCADE,
	FOREIGN KEY (CadSecteurNumero)
	REFERENCES Cadastre.CadastraleDivisions(Numero) ON UPDATE CASCADE
)
GO
create Spatial index SpInd_JuridiqueObjets
	on Cadastre.JuridiqueObjets (Shape)
	with (Bounding_Box = (XMIN = 250000, XMAX = 595000, YMIN = 690000, YMAX = 1370000));