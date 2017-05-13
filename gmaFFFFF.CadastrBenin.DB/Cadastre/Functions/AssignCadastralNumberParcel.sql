-- =============================================
-- Description:	Присваивает кадастровый номер новому земельному участку.
-- В перспективе необходимо переделать на центрального раздатчика кадастровых номеров
-- =============================================
create function Cadastre.AssignCadastralNumberParcel
(
	@borderPgn geometry
)
returns varchar (25)
as
begin
	if @borderPgn is null
		return null;
		
		
	declare @ParcelCadNum varchar(25);
	
	declare @CadSector varchar(8) = Cadastre.FindParentCadSector(@borderPgn);
	declare @ParcelNumInSector int = Cadastre.GenerateNextParcelNumber(@CadSector);
	set @ParcelCadNum = Cadastre.ExtractParcelCadastralNumber(@CadSector + '-' + cast (@ParcelNumInSector as varchar (17)));
	
	return @ParcelCadNum;
end