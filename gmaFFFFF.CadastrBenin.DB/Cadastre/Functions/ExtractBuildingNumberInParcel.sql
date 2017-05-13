-- =============================================
-- Description:	Выделяет номер здания на земельном участке из его кадастрового номера
-- =============================================
CREATE FUNCTION Cadastre.ExtractBuildingNumberInParcel
(
	@buildCadNumber varchar(30)
)
RETURNS varchar(25)
AS
BEGIN	
	if @buildCadNumber is null
		return null;
		
	DECLARE @BuildingNum int;

	select @BuildingNum = BuildingInd
	from Cadastre.ParseCadastralNumber(@buildCadNumber);
	
	
	RETURN @BuildingNum

END