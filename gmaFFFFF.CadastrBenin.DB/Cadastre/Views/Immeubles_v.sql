CREATE VIEW Cadastre.Immeubles_v AS
SELECT Im.NUP, Im.Nom, Im.PiecesNombre, Im.EtagesNombre, Ju.Shape, Ju.Superficie
FROM Cadastre.JuridiqueObjets Ju, Cadastre.Immeubles Im
WHERE Im.GUID = Ju.GUID  and 
JuridiqueObjetTypeNom = N'Immeuble'
WITH CHECK OPTION