-- =============================================
-- Description:	Определяет следующий порядковый номер земельного участка в кадастровом районе
-- =============================================
create function Cadastre.GenerateNextParcelNumber
(
	@sectorNumber varchar(8)
)
returns int
as
begin
	if @sectorNumber is null
		return null;
	declare @ParcelNumber int;
	
	select @ParcelNumber = max(P.ParcelleSurCadSecteurNumero) + 1
	from   Cadastre.JuridiqueObjets  as O
		   join Cadastre.Parcelles   as P
				on  O.GUID = P.GUID and
					O.JuridiqueObjetTypeNom = N'Parcelle'
	where  O.CadSecteurNumero = @sectorNumber
	group by O.CadSecteurNumero;
	
	if @ParcelNumber is null
		set @ParcelNumber = 1;
	
	return @ParcelNumber;
end