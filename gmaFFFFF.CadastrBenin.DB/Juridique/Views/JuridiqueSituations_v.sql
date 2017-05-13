CREATE VIEW Juridique.JuridiqueSituations_v AS
SELECT Pa.NUP, TranT.JuridiqueTypeNom, Su.GUID ProprietarePresume, Sp.NomEtPrenome as Nom, Tr.Observations
FROM Juridique.TransactionTypes TranT, Cadastre.JuridiqueObjets Ju, Sujet.Sujets Su, Juridique.ProprietairePresume Pr, Juridique.Transactions Tr, Cadastre.Parcelles Pa, Juridique.TransactionObjets Tra, Sujet.Personnes SP
WHERE Pr.SujetGUID = Su.GUID AND Tr.TransactionTypeNom = TranT.Nom AND Pr.TransactionGUID = Tr.GUID AND Pa.GUID = Ju.GUID AND Tra.JuridiqueObjetGUID = Ju.GUID AND Tra.TransactionGUID = Tr.GUID and SP.GUID = Su.GUID
and Tr.FinDate is null
union all
SELECT Pa.NUP, TranT.JuridiqueTypeNom, Su.GUID ProprietarePresume, SC.Nom as Nom, Tr.Observations
FROM Juridique.TransactionTypes TranT, Cadastre.JuridiqueObjets Ju, Sujet.Sujets Su, Juridique.ProprietairePresume Pr, Juridique.Transactions Tr, Cadastre.Parcelles Pa, Juridique.TransactionObjets Tra, Sujet.Corporations SC
WHERE Pr.SujetGUID = Su.GUID AND Tr.TransactionTypeNom = TranT.Nom AND Pr.TransactionGUID = Tr.GUID AND Pa.GUID = Ju.GUID AND Tra.JuridiqueObjetGUID = Ju.GUID AND Tra.TransactionGUID = Tr.GUID and SC.GUID = Su.GUID
and Tr.FinDate is null