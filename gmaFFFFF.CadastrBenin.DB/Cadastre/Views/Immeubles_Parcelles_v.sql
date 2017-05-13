CREATE VIEW Cadastre.Immeubles_Parcelles_v AS
	with ParcelImm
	as
	(
		select	Pa.NUP as ParcelNUP, I.NUP as ImmeubleNUP
	
		from		(select Pa.NUP, jo.Shape
			  		from Cadastre.Parcelles as Pa join Cadastre.JuridiqueObjets as jo on jo.GUID = Pa.GUID) as Pa 
		left join	(select I.NUP, jo.Shape
				 	 from Cadastre.Immeubles as I join Cadastre.JuridiqueObjets as jo on jo.GUID = I.GUID) as I 
			on Pa.Shape.STIntersects(I.Shape) = 1
	)
	select	ParcelNUP, count(ImmeubleNUP) as ImmeublesCount
	from ParcelImm
	group by ParcelNUP