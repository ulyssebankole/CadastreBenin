CREATE VIEW Cadastre.CadastraleSecteurs_v AS
SELECT Ca.Numero, Ca.Id, Ca.Nom, Ca.Mere, Ca.Shape
FROM Cadastre.CadastraleDivisions Ca
WHERE Niveau = 3
WITH CHECK OPTION