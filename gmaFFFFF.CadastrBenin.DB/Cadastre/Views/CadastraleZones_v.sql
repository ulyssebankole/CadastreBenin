CREATE VIEW Cadastre.CadastraleZones_v AS
SELECT Ca.Numero, Ca.Id, Ca.Nom, Ca.Mere, Ca.Shape
FROM Cadastre.CadastraleDivisions Ca
WHERE Niveau = 2
WITH CHECK OPTION