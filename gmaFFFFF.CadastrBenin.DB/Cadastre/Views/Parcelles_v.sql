CREATE VIEW Cadastre.Parcelles_v AS
SELECT Pa.NUP, Pa.SiArpentageFrontiere, Ju.Shape, Ju.Superficie
FROM Cadastre.JuridiqueObjets Ju, Cadastre.Parcelles Pa
WHERE Pa.GUID = Ju.GUID and 
JuridiqueObjetTypeNom = N'Parcelle'
WITH CHECK OPTION