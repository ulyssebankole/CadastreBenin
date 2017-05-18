# Прототип системы кадастрового учета Республики Бенин

## Общие сведения
_Язык программирования_: C#, T-SQL
 
_Среда разработки_: MS Visual Studio 2015

_Лицензия_: MIT License, Copyright © 2017 Ulysse BANKOLE (ulyssebankole@yahoo.fr)

_Ключевые слова_: кадастр, БД, Республика Бенин, земельные участки, регистрация прав

## Структура проекта
|№   	|Название   						|Описание   		|
|---	|---								|---	|	
|1.  	|gmaFFFFF.CadastrBenin.DB   		|Создает БД MS SQL Server (уровень данных)  				|
|2.  	|gmaFFFFF.CadastrBenin.DAL   		|[ORM](https://ru.wikipedia.org/wiki/ORM) - Entity Framework (уровень доступа к данным)					|
|3.  	|gmaFFFFF.CadastrBenin.ViewModel   	|Модель представления (уровень бизнес-логики)   			|
|4. 	|gmaFFFFF.CadastrBenin.DesktopApp   |Десктопное WPF приложение (уровень представления)  		|
|5.  	|gmaFFFFF.CadastrBenin.Deploy.Settings|Настраивает приложение сразу после его чистой установки  |
|6.  	|gmaFFFFF.CadastrBenin.Deploy   	|Создает инсталлятор приложения в среде Advanced Installer	|

## Документация

Документация размещена в каталоге [CadastreBenin/Doc](https://github.com/ulyssebankole/CadastreBenin/tree/master/Doc)

## Системные требования
1. Microsoft Windows 7 или выше
2. Microsoft .Net Framework 4.5.2 (автоустановка)
3. MS SQL Server Express LocalDB 2016 (автоустановка)
4. MS ODBC Driver 13 for SQL Server (автоустановка)
5. Пакет дополнительных компонентов MS SQL Server - SQLSysClrTypes (автоустановка)
