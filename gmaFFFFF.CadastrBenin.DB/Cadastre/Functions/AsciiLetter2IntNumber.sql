-- =============================================
-- Description:	Перекодирует число в 26-ричной системе счисления, отображенное с помощью букв, в десятичную систему счисления
-- =============================================
Create function Cadastre.AsciiLetter2IntNumber
(
	@asciiLetter varchar(10)
)
returns int
with schemabinding
as

begin
	if @asciiLetter is null
		return null;
	
	declare @MaxLetter char = 'z';
	declare @MinLetter char = 'a';
	declare @IntNumber int = 0;												--Закодированное число
	declare @Radix tinyint = ascii(@MaxLetter) - ascii(@MinLetter) + 1;		--Основание системы счисления
	declare @Len tinyint = len(@asciiLetter);
	declare @rank tinyint = 0
	
	WHILE @Len >= 1 
	begin
		declare @Symbol char = substring(@asciiLetter, len(@asciiLetter) - @rank, 1)
		if @rank = 0
			set @IntNumber = @IntNumber + (ascii(@Symbol) - ascii(@MinLetter)) * power(@Radix, @rank);
		else 
			set @IntNumber = @IntNumber + (ascii(@Symbol) - ascii(@MinLetter) + 1) * power(@Radix, @rank);
		set @rank = @rank + 1;
		set @Len = @Len - 1;
	END
	
	
	
	return @IntNumber
end