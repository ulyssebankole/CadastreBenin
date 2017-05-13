CREATE VIEW Cadastre.CadastraleDepartements_v AS
SELECT Ca.Numero, Ca.Id, Ca.Nom, Ca.Mere, Ca.Shape
FROM Cadastre.CadastraleDivisions Ca
WHERE Niveau = 1
WITH CHECK OPTION