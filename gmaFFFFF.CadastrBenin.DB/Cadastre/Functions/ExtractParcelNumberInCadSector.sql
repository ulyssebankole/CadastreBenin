-- =============================================
-- Description:	Выделяет номер земельного участка в кадастровом районе из его кадастрового номера
-- =============================================
CREATE FUNCTION Cadastre.ExtractParcelNumberInCadSector
(
	@parcelCadNumber varchar(30)
)
RETURNS int
AS
BEGIN	
	if @parcelCadNumber is null
		return null;
		
	DECLARE @ParcelNum int;

	select @ParcelNum = ParcelInd
	from Cadastre.ParseCadastralNumber(@parcelCadNumber);
	
	
	RETURN @ParcelNum

END