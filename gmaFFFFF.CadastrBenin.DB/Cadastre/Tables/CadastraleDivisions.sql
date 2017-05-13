/*
 * ER/Studio Data Architect 10.0 SQL Code Generation
 * Project :	  Benine.DM1
 *
 * Date Created : Sunday, April 16, 2017 23:40:21
 * Target DBMS : Microsoft SQL Server 2014
 */



CREATE TABLE Cadastre.CadastraleDivisions(
	Numero		AS			   (isnull(Cadastre.CadastrDevisionHierarchyid2Name(Hierarchie),'')) PERSISTED NOT NULL,
	Nom		   nvarchar(100)	NULL,
	Hierarchie	hierarchyid	  NOT NULL,
	Id			AS			   cast(Replace(Replace(Hierarchie.ToString(), Hierarchie.GetAncestor(1).ToString(),''),'/','') as tinyint) PERSISTED NOT NULL,
	Mere		  AS			   Hierarchie.GetAncestor(1) PERSISTED,
	Niveau		AS			   Hierarchie.GetLevel() PERSISTED,
	Shape		 geometry		 NOT NULL,
	PRIMARY KEY CLUSTERED (Numero),
	UNIQUE (Hierarchie)
)
GO
create Spatial index SpInd_CadastraleDivisions
	on Cadastre.CadastraleDivisions (Shape)
	with (Bounding_Box = (XMIN = 250000, XMAX = 595000, YMIN = 690000, YMAX = 1370000));