-- =============================================
-- Description:	Кодирует число в 26-ричную систему счисления и отображает его с помощью букв
-- =============================================
create function Cadastre.IntNumber2AsciiLetter
(
	@intNumber int
)
returns varchar(10)
with schemabinding
as

begin
	if @intNumber is null
		return null;
		
	declare @MaxLetter char = 'z';
	declare @MinLetter char = 'a';
	declare @AsciiLetter varchar(10);									--Закодированное число
	declare @Radix tinyint = ascii(@MaxLetter) - ascii(@MinLetter) + 1;	--Основание системы счисления
	declare @Remainder int = @intNumber;								--Остаток
	declare @Quotient int;												--Частное от деления
	
	set @Quotient = @Remainder % @Radix;
	set @AsciiLetter = char(ascii(@MinLetter) + @Quotient);
	set @Remainder = floor(@Remainder / @Radix);
	
	
	while @Remainder <> 0
	begin		
		set @Remainder = @Remainder - 1;
		set @Quotient = @Remainder % @Radix;
		set @AsciiLetter = char(ascii(@MinLetter) + @Quotient) + @AsciiLetter;
		set @Remainder = floor(@Remainder / @Radix);
	end
	
	return @AsciiLetter
end