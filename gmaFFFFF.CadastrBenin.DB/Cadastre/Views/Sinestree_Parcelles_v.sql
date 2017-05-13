CREATE VIEW Cadastre.Sinestree_Parcelles_v AS
SELECT Pa.NUP, Si.SinistreeTypeNom, Si.Influence, Si.InfluenceCoefficient, Sum(Pa.Shape.STIntersection(Si.Shape).STArea()) as Area
FROM Cadastre.SinistreeRegions Si join (select Pa.NUP, jo.Shape 
										from Cadastre.Parcelles Pa join Cadastre.JuridiqueObjets as jo on jo.GUID = Pa.GUID) as Pa on Si.Shape.STIntersects(Pa.Shape) = 1
group by Pa.NUP, Si.SinistreeTypeNom, Si.Influence, Si.InfluenceCoefficient