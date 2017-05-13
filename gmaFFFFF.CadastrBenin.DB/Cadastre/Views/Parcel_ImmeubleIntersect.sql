CREATE VIEW [Cadastre].[Parcel_ImmeubleIntersect]
	AS 
	select isnull(ROW_NUMBER() over (order by ParcelNUP, ImmeubleNUP), 0) as Id, ParcelNUP, ImmeubleNUP, ParcelShape, ImmeubleShape 
	from(SELECT NUP as ParcelNUP, Shape as ParcelShape
		 FROM Cadastre.Parcelles as P join Cadastre.JuridiqueObjets as O on P.GUID = O.GUID) as P
	join(SELECT NUP as ImmeubleNUP, Shape as ImmeubleShape
		 FROM Cadastre.Immeubles as I join Cadastre.JuridiqueObjets as O on I.GUID = O.GUID) as I
	on ParcelShape.STIntersects(ImmeubleShape) = 1
