-- =============================================
-- Description:	Находит земельный участок в котором расположен полигон (здание) или его центроид
-- =============================================
create function Cadastre.FindParentParcel
(
	@borderPgn geometry
)
returns varchar (25)
as
begin
	if @borderPgn is null
		return null;
	
	declare @CadSector varchar(8) = Cadastre.FindParentCadSector(@borderPgn);
	
	if @CadSector is null
		return null;
	
	declare @ParcelNUP varchar(25);
/*Данный код снижает производительность запроса, но повышает его точность
	with Parcel
	as
	(
		select P.NUP, O.Shape
		from   Cadastre.JuridiqueObjets  as O
			   join Cadastre.Parcelles   as P
					on  O.JuridiqueObjetTypeNom = N'Parcelle' and
						O.CadSecteurNumero = @CadSector and
						P.GUID = O.GUID
		
	)
	select @ParcelNUP = P.NUP
	from Parcel as P	
	where Exists (select 1 
				  where P.Shape.STContains(@borderPgn) = 1);	--Целиком границу объекта
	
	
	
	if @ParcelNUP is null*/
		with Parcel
		as
		(
			select P.NUP, O.Shape
			from   Cadastre.JuridiqueObjets  as O
				   join Cadastre.Parcelles   as P
						on  O.JuridiqueObjetTypeNom = N'Parcelle' and
							O.CadSecteurNumero = @CadSector and
							P.GUID = O.GUID
			
		)
		select @ParcelNUP = P.NUP
		from Parcel as P	
		where Exists (select 1 
					  where P.Shape.STContains(@borderPgn.STCentroid()) = 1);		--Хотя бы центроид

	
	return @ParcelNUP;
end