-- =============================================
-- Description:	Определяет следующий порядковый номер здания на земельном участке
-- =============================================
create function Cadastre.GenerateNextBuildingNumber
(
	@parcelCadastralNumber varchar(25)
)
returns int
as
begin
	if @parcelCadastralNumber is null
		return null;
	declare @BuildingNumber int;
	
	select @BuildingNumber = max(ImmeubleSurParcelleNumero) + 1
	from   Cadastre.Immeubles
	where  ParcelleNUP = @parcelCadastralNumber
	group by ParcelleNUP
	
	if @BuildingNumber is null
		set @BuildingNumber = 0;
	
	return @BuildingNumber;
end