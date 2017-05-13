-- =============================================
-- Description: Конвертирует hierarchyid в название единицы кадастрового деления
-- =============================================
CREATE FUNCTION Cadastre.CadastrDevisionHierarchyid2Name 
(
@h hierarchyid
)
RETURNS varchar(8)
with Schemabinding
AS
BEGIN
	--Функция Format является недетерминированной поэтому следующий код придется выполнять с помощью условного оператора	
	--declare @num varchar(8) = format(cast(Replace(Replace(@h.ToString(), @h.GetAncestor(1).ToString(),''),'/','') as tinyint), '00', 'en-us');
	declare @num varchar(8) = Replace(Replace(@h.ToString(), @h.GetAncestor(1).ToString(),''),'/','');
	if len(@num) < 2
		set @num = '0' + @num;
	
	while @h.GetLevel() > 1
	begin
		set @h = @h.GetAncestor(1);
		
		declare @n varchar(8) = Replace(Replace(@h.ToString(), @h.GetAncestor(1).ToString(),''),'/','');
		if len(@n) < 2
			set @n = '0' + @n;
		
		set @num = @n + '-' + @num;
	end
	
	RETURN @num
END