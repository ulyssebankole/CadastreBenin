-- =============================================
-- Description:	Распознает элементы кадастрового номера
-- =============================================
CREATE FUNCTION Cadastre.ParseCadastralNumber 
(
	@cadNumber varchar(30)
)
RETURNS 
@CadNumParse table (DepartmentInd tinyint, ZoneInd tinyint, SectorInd tinyint, ParcelInd int, BuildingInd int,
					ZoneNum char(5), SectorNum char(8), ParcelNum varchar(25), BuildingNum varchar (30))
with schemabinding

AS
begin
	if @cadNumber is null
		return;
		
	declare @CadastralNumberRegEx varchar (150) = N'(?<Department>[0-9]{1,2})(-(?<Zone>[0-9]{1,2}))?(-(?<Sector>[0-9]{1,2}))?(-(?<Parcel>[0-9]+))?(-(?<Building>[a-z]+))?';
	declare	@DepartmentInd	tinyint, 
			@ZoneInd		tinyint, 
			@SectorInd		tinyint, 
			@ParcelInd		int, 
			@BuildingInd	int,
			@ZoneNum		char(5), 
			@SectorNum		char(8), 
			@ParcelNum		varchar(25), 
			@BuildingNum	varchar (30);
		
	with Parce
	as
	(
		select	case Groups.A.value(N'@номер','tinyint') 
					when 5 then 'DepartmentInd'
					when 6 then 'ZoneInd'
					when 7 then 'SectorInd'
					when 8 then 'ParcelInd'
					when 9 then 'BuildingInd'
				end as Ind, 
				nullif(Groups.A.value('.','varchar(30)'),'') as Val
		from Services.RegExMatches(@CadastralNumberRegEx, @cadNumber, null) as rem 
		cross apply rem.groups.nodes(N'/группа') as Groups (A)
		where Groups.A.value(N'@номер','tinyint') in (5,6,7,8,9)
	)
	select	@DepartmentInd	= (select try_cast(Val as tinyint)	from Parce where Ind = 'DepartmentInd'),
			@ZoneInd		= (select try_cast(Val as tinyint)	from Parce where Ind = 'ZoneInd'),
			@SectorInd		= (select try_cast(Val as tinyint)	from Parce where Ind = 'SectorInd'),
			@ParcelInd		= (select try_cast(Val as int)		from Parce where Ind = 'ParcelInd'),
			@BuildingInd	= (select Cadastre.AsciiLetter2IntNumber(Val) from Parce where Ind = 'BuildingInd');
			
	/*Данный код ясный, но не детерминирован
	select	@ZoneNum		= format(@DepartmentInd,'00') + '-' + format(@ZoneInd,'00'),
			@SectorNum		= format(@DepartmentInd,'00') + '-' + format(@ZoneInd,'00') + '-' + format(@SectorInd,'00'),
			@ParcelNum		= format(@DepartmentInd,'00') + '-' + format(@ZoneInd,'00') + '-' + format(@SectorInd,'00') + '-' + format(@ParcelInd,'0000'),
			@BuildingNum	= format(@DepartmentInd,'00') + '-' + format(@ZoneInd,'00') + '-' + format(@SectorInd,'00') + '-' + format(@ParcelInd,'0000') + '-' + Cadastre.IntNumber2AsciiLetter(@BuildingInd);
	*/
	--Детерминированный эквивалент предыдущего кода
	declare @dep varchar(2)		= cast (@DepartmentInd as varchar(2));
	declare @Zone varchar(2)	= cast (@ZoneInd as varchar(2));
	declare @Sect varchar(2)	= cast (@SectorInd as varchar(2));
	declare @parc varchar(16)	= cast (@ParcelInd as varchar(16));
	if len (@dep) < 2
		set @dep = '0' + @dep;
	if len (@Zone) < 2
		set @Zone = '0' + @Zone;
	if len (@Sect) < 2
		set @Sect = '0' + @Sect;
	if len (@parc) < 2
		set @parc = '000' + @parc;
	else if len (@parc) < 3
		set @parc = '00' + @parc;	
	else if len (@parc) < 4
		set @parc = '0' + @parc;	
	select	@ZoneNum		= @dep + '-' + @Zone,
			@SectorNum		= @dep + '-' + @Zone + '-' + @Sect,
			@ParcelNum		= @dep + '-' + @Zone + '-' + @Sect + '-' + @parc,
			@BuildingNum	= @dep + '-' + @Zone + '-' + @Sect + '-' + @parc + '-' + Cadastre.IntNumber2AsciiLetter(@BuildingInd);
	--Конец эквивалента
	
	
	
	insert into @CadNumParse (DepartmentInd, ZoneInd, SectorInd, ParcelInd, BuildingInd, ZoneNum, SectorNum, ParcelNum, BuildingNum)
		values (@DepartmentInd, @ZoneInd, @SectorInd, @ParcelInd, @BuildingInd, @ZoneNum, @SectorNum, @ParcelNum, @BuildingNum)
	
	RETURN 
END