-- =============================================
-- Description:	Выделяет кадастровый номер земельного участка из кадастрового номера здания
-- =============================================
CREATE FUNCTION Cadastre.ExtractParcelCadastralNumber 
(
	@buildCadNumber varchar(30)
)
RETURNS varchar(25)
with schemabinding
AS
BEGIN	
	if @buildCadNumber is null
		return null;
		
	DECLARE @ParcelCadNumber varchar (25);

	select @ParcelCadNumber = ParcelNum
	from Cadastre.ParseCadastralNumber(@buildCadNumber);
	
	
	RETURN @ParcelCadNumber

END