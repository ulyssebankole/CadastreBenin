-- =============================================
-- Description:	Выделяет номер кадастрового сектора из кадастрового номера объекта
-- =============================================
CREATE FUNCTION Cadastre.ExtractCadSectorNumber 
(
	@JurObjectCadNumber varchar(30)
)
RETURNS varchar(8)
AS
BEGIN	
	if @JurObjectCadNumber is null
		return null;
		
	DECLARE @CadSectorNumber varchar (8);

	select @CadSectorNumber = SectorNum
	from Cadastre.ParseCadastralNumber(@JurObjectCadNumber);
	
	
	RETURN @CadSectorNumber

END