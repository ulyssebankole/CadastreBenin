CREATE TABLE Cadastre.SinistreeRegions(
	Id				  int			 IDENTITY(1,1),
	SinistreeTypeNom	nvarchar(50)	NOT NULL,
	Influence		   nvarchar(25)	NULL,
    InfluenceCoefficient    float           NULL,
	Shape			   geometry		NOT NULL,
	PRIMARY KEY CLUSTERED (Id), 
	FOREIGN KEY (SinistreeTypeNom)
	REFERENCES Cadastre.SinistreeTypes(Nom) ON UPDATE CASCADE
)
GO
create Spatial index SpInd_SinistreeRegions
	on Cadastre.SinistreeRegions (Shape)
	with (Bounding_Box = (XMIN = 250000, XMAX = 595000, YMIN = 690000, YMAX = 1370000));