CREATE TABLE Sujet.SujetDocument(
	Id					  int				 IDENTITY(1,1),
	SujetGUID			   uniqueidentifier	NOT NULL,
	SujetDocumentTypeNom	nvarchar(50)		NOT NULL,
	Date					date				NULL,
	Num					 varchar(10)		 NULL,
	PRIMARY KEY NONCLUSTERED (Id), 
	FOREIGN KEY (SujetGUID)
	REFERENCES Sujet.Sujets(GUID) ON DELETE CASCADE,
	FOREIGN KEY (SujetDocumentTypeNom)
	REFERENCES Sujet.SujetDocumentTypes(Nom) ON UPDATE CASCADE
)
GO
CREATE CLUSTERED INDEX CInd_SujetDocument ON Sujet.SujetDocument(SujetGUID)