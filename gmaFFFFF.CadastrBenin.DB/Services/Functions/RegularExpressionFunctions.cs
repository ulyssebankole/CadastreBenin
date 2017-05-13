using System;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

namespace gmaFFFFF.SQLServer.RegEx
{
	/// <summary>
	/// Класс обеспечивает ограниченную функциональность регулярных выражений для SQL Server
	/// </summary>
	/// <remarks><para>Класс построен путем переписывания CLR Assembly RegEx Functions for SQL Server на язык C#. Автор Phil Factor. и небольшой его модификации</para>
	/// <para>https://www.simple-talk.com/sql/t-sql-programming/clr-assembly-regex-functions-for-sql-server-by-example/</para></remarks>
	public partial class RegularExpressionFunctions
	{
		/// <summary>Функция RegExIsMatch позволяет проводить проверку соответствия строки (<paramref name="input"/>) заданному регулярному выражению (<paramref name="pattern"/>).
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Сообщает найдено ли совпадение. Если совпадение найдено, то возвращает true</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExIsMatch", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlBoolean RegExIsMatch(SqlString pattern, SqlString input, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return SqlBoolean.False;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			
			return Regex.IsMatch(input.Value, pattern.Value, options);
		}

		/// <summary>
		/// Преобразует минимальный набор символов (\, *, +, ?, |, {, [, (,), ^, $,., # и пробел), заменяя их соответствующими escape-кодами. При этом обработчику регулярных выражений дается команда интерпретировать эти символы буквально, а не как метасимволы.
		/// </summary>
		/// <param name="input">Входная строка, содержащая текст для преобразования.</param>
		/// <returns>Строка символов с метасимволами, приведенными в преобразованную форму.</returns>
		/// <remarks>Escape Преобразует строку таким образом, обработчик регулярных выражений интерпретирует все метасимволы, он может содержать как символьные литералы.</remarks>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExEscape", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlString RegExEscape(SqlString input)
		{
			if (input.IsNull)
			{
				return string.Empty;
			}
			return Regex.Escape(input.Value);
		}

		/// <summary>
		/// Ищет во входной строке (<paramref name="input"/>) первое вхождение регулярного выражения (<paramref name="pattern"/>) и возвращает его позицию.
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Позиция в исходной строке, в которых находится первый символ захваченной части строки.</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExIndex", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlInt32 RegExIndex(SqlString pattern, SqlString input, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return 0;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			return Regex.Match(input.Value, pattern.Value, options).Index;
		}

		/// <summary>
		/// Ищет во входной строке (<paramref name="input"/>) первое вхождение регулярного выражения (<paramref name="pattern"/>) и возвращает его.
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Первое вхождение регулярного выражения или пустую строкую</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExMatch", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlString RegExMatch(SqlString pattern, SqlString input, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return string.Empty;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			return Regex.Match(input.Value, pattern.Value, options).Value;
		}

		/// <summary>
		/// Ищет во входной строке (<paramref name="input"/>) все вхождения регулярного выражения (<paramref name="pattern"/>) и возвращает все соответствия.
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Коллекция Match объектов, найденных при поиске. Если соответствующие объекты не найдены, метод возвращает пустой объект коллекции.</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExMatches", 
			SystemDataAccess = SystemDataAccessKind.None, FillRowMethodName = "NextMatchedRow",
			TableDefinition = "match nvarchar(max), matchIndex int, matchLength int, groups xml")]
		public static IEnumerable RegExMatches(SqlString pattern, SqlString input, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return null;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			return Regex.Matches(input.Value, pattern.Value, options);
		}

		/// <summary>
		/// Используется для создания битовой маски, которая используется различными функциями текущего класса
		/// </summary>
		/// <param name="IgnoreCase">Указывает соответствие, не учитывающее регистр. </param>
		/// <param name="MultiLine">Многострочный режим. Изменяет значение символов "^" и "$" так, что они совпадают, соответственно, в начале и конце любой строки, а не только в начале и конце целой строки. </param>
		/// <param name="ExplicitCapture">Указывает, что единственные допустимые записи являются явно поименованными или пронумерованными группами в форме <![CDATA[(?<name>…)]]>. Это позволяет использовать непоименованные круглые скобки в качестве незахватываемых групп, тем самым не допуская синтаксической громоздкости выражения (?:…).</param>
		/// <param name="Compiled">Указывает, что регулярное выражение скомпилировано в сборку. Это дает более быстрое исполнение, но увеличивает время запуска. </param>
		/// <param name="SingleLine">Указывает однострочный режим. Изменяет значение точки (.) поэтому она соответствует любому символу (вместо любого символа, за исключением \n)</param>
		/// <param name="IgnorePatternWhitespace">Устраняет из шаблона разделительные символы без escape-последовательности и включает комментарии, помеченные символом "#". Однако это значение не влияет на или устранить пробелы в, числовые или токенах, отмечающих начало отдельных. </param>
		/// <param name="RightToLeft">Указывает, что поиск будет выполнен в направлении справа налево, а не слева направо. </param>
		/// <param name="ECMAScript">Включает ECMAScript-совместимое поведение для выражения. Это значение может использоваться только в сочетании с IgnoreCase, Multiline, и Compiled значения. Использование этого значения вместе с любыми другими приводит к исключению.</param>
		/// <param name="CultureInvariant">Указывает игнорирование региональных языковых различий.</param>
		/// <returns>Число int, представляющее собой битовую маску с установленными RegexOptions</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExOptionEnumeration", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlInt32 RegExOptionEnumeration(SqlBoolean IgnoreCase,
			SqlBoolean MultiLine,
			SqlBoolean ExplicitCapture,
			SqlBoolean Compiled,
			SqlBoolean SingleLine,
			SqlBoolean IgnorePatternWhitespace,
			SqlBoolean RightToLeft,
			SqlBoolean ECMAScript,
			SqlBoolean CultureInvariant)
		{
			return	(int) (IgnoreCase.IsTrue		? RegexOptions.IgnoreCase		: RegexOptions.None) |
					(int) (MultiLine.IsTrue			? RegexOptions.Multiline		: RegexOptions.None) |
					(int) (ExplicitCapture.IsTrue	? RegexOptions.ExplicitCapture	: RegexOptions.None) |
					(int) (Compiled.IsTrue			? RegexOptions.Compiled			: RegexOptions.None) |
					(int) (SingleLine.IsTrue		? RegexOptions.Singleline		: RegexOptions.None) |
					(int) (IgnorePatternWhitespace.IsTrue ? RegexOptions.IgnorePatternWhitespace : RegexOptions.None) |
					(int) (RightToLeft.IsTrue		? RegexOptions.RightToLeft		: RegexOptions.None) |
					(int) (ECMAScript.IsTrue		? RegexOptions.ECMAScript		: RegexOptions.None) |
					(int) (CultureInvariant.IsTrue	? RegexOptions.CultureInvariant : RegexOptions.None);
		}

		/// <summary>
		/// В указанной входной строке (<paramref name="input"/>) заменяет все строки, соответствующие шаблону регулярного выражения (<paramref name="pattern"/>), указанной строкой замены.
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="replacement">Строка на которую будет осуществлена замена</param>
		/// <returns>Новая строка, идентичная входной строке (<paramref name="input"/>), за исключением того что строка замены (<paramref name="replacement"/>) занимает место каждой соответствующей шаблону (<paramref name="pattern"/>) строки. Если в текущем экземпляре нет соответствия шаблону регулярных выражений (<paramref name="pattern"/>), метод возвращает текущий экземпляр (<paramref name="input"/>) без изменений.</returns>
		/// <remarks>Применяются опции RegexOptions.Multiline (многострочный режим) | RegexOptions.IgnoreCase (без учета регистра)</remarks>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExReplace", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlString RegExReplace(SqlString input, SqlString pattern, SqlString replacement)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(Regex.Replace(input.Value, pattern.Value, replacement.Value, RegexOptions.Multiline | RegexOptions.IgnoreCase));
		}

		/// <summary>
		/// В указанной входной строке (<paramref name="input"/>) заменяет все строки, соответствующие шаблону регулярного выражения (<paramref name="pattern"/>), указанной строкой замены.
		/// </summary>
		/// <param name="input">Строка для поиска соответствия</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="replacement">Строка на которую будет осуществлена замена</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Новая строка, идентичная входной строке (<paramref name="input"/>), за исключением того что строка замены (<paramref name="replacement"/>) занимает место каждой соответствующей шаблону (<paramref name="pattern"/>) строки. Если в текущем экземпляре нет соответствия шаблону регулярных выражений (<paramref name="pattern"/>), метод возвращает текущий экземпляр (<paramref name="input"/>) без изменений.</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExReplacex", SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlString RegExReplacex(SqlString pattern, SqlString input, SqlString replacement, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return SqlString.Null;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			return new SqlString(Regex.Replace(input.Value, pattern.Value, replacement.Value, options));
		}

		/// <summary>
		/// Разделяет входную строку (<paramref name="input"/>) в массив подстрок в позициях, определенных шаблоном регулярного выражения (<paramref name="pattern"/>).
		/// </summary>
		/// <param name="input">Строка для разделения</param>
		/// <param name="pattern">Шаблон регулярного выражения</param>
		/// <param name="Options">Пользовательские настройки регулярного выражения, можно сгенирировать фукнцией <see cref="RegExOptionEnumeration"/></param>
		/// <returns>Массив строк</returns>
		[SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "RegExSplit", 
			SystemDataAccess = SystemDataAccessKind.None, FillRowMethodName = "NextSplitRow",
			TableDefinition = "string nvarchar(max)")]
		public static IEnumerable RegExSplit(SqlString pattern, SqlString input, SqlInt32 Options)
		{
			if (input.IsNull || pattern.IsNull)
			{
				return null;
			}
			RegexOptions options = Options.IsNull ? RegexOptions.None : (RegexOptions)((int)Options);
			return Regex.Split(input.Value, pattern.Value, options);
		}

		/// <summary>
		/// Вспомогательная функция, предназначена для передачи SQL Server отдельных значений строки, получаемой в ходе обработки <see cref="RegExSplit"/>.
		/// </summary>
		/// <param name="input">Класс Match. Представляет результаты из отдельного совпадения регулярного выражения.</param>
		/// <param name="match">Получает захваченную подстроку из входной строки.</param>
		/// <param name="matchIndex">Позиция в исходной строке, в которых находится первый символ захваченной части строки.</param>
		/// <param name="matchLength">Возвращает длину захваченной части строки.</param>
		/// <param name="groups">Возвращает перечень распознанных групп в формате XML 
		/// <code><группа номер = "1">совпадения</группа></code></param>
		private static void NextMatchedRow(object input, out SqlString match, out SqlInt32 matchIndex, out SqlInt32 matchLength, out SqlXml groups)
		{
			Match match2 = (Match)input;
			match = new SqlString(match2.Value);
			matchIndex = new SqlInt32(match2.Index);
			matchLength = new SqlInt32(match2.Length);

			StringBuilder Gr = new StringBuilder();
			for (int i = 1; i < match2.Groups.Count; i++)
			{
				Gr.Append("<группа номер = \"").Append(i).Append("\">").Append(match2.Groups[i]).Append("</группа>");
			}
			
			if (Gr.Length > 0)
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.ConformanceLevel = ConformanceLevel.Fragment;

				using (StringReader sr = new StringReader(Gr.ToString()))
				using (XmlReader xr = XmlReader.Create(sr, settings))
				{
					groups = new SqlXml(xr);
				}
			}
			else
				groups = SqlXml.Null;
		}
		/// <summary>
		///  Вспомогательная функция, предназначена для передачи SQL Server отдельных значений строки, получаемой в ходе обработки <see cref="RegExSplit"/>.
		/// </summary>
		/// <param name="input">строка</param>
		/// <param name="match">строка типа SqlString</param>
		private static void NextSplitRow(object input, out SqlString match)
		{
			match = new SqlString((string)input);
		}
	}
}