-- =============================================
-- Description:	Присваивает кадастровый номер новому зданию.
-- В перспективе необходимо переделать на центрального раздатчика кадастровых номеров
-- =============================================
create function Cadastre.AssignCadastralNumberBuilding
(
	@borderPgn geometry
)
returns varchar (25)
as
begin
	if @borderPgn is null
		return null;
		
		
	declare @BuildingCadNum varchar(30);
	
	declare @Parcel varchar(25) = Cadastre.FindParentParcel(@borderPgn);
	declare @BuildingNumInParcel int = Cadastre.GenerateNextBuildingNumber(@Parcel);
	set @BuildingCadNum = @Parcel + '-' + Cadastre.IntNumber2AsciiLetter(@BuildingNumInParcel);
	
	return @BuildingCadNum;
end