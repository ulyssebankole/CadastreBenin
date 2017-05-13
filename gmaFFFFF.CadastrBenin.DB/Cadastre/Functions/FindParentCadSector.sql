-- =============================================
-- Description:	Находит кадастровый район в котором расположен полигон (земельный участок) или его центроид
-- =============================================
CREATE FUNCTION Cadastre.FindParentCadSector 
(
	@borderPgn geometry
)
RETURNS varchar(8)
AS
BEGIN
	if @borderPgn is null
		return null;
		
	DECLARE @CadSector varchar(8);

	select @CadSector = Numero
	from Cadastre.CadastraleSecteurs_v
	where Shape.STContains(@borderPgn) = 1;
	
	if @CadSector is null
		select @CadSector = Numero
		from Cadastre.CadastraleSecteurs_v
		where Shape.STContains(@borderPgn.STCentroid()) = 1;
	
	RETURN @CadSector;

END