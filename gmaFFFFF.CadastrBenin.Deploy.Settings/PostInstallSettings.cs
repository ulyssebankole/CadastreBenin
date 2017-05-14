using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace gmaFFFFF.CadastrBenin.Deploy.Settings
{
	/// <summary>
	/// Осуществляет подготовку основной программы к запуску после установки
	/// </summary>
	class PostInstallSettings
	{
		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("Активация режима поддержки Clr");
				EnableDataBaseClr();

				Console.WriteLine("Обновляем подключение к БД");
				string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				UpdateConnectionString(currentPath + @"\gmaFFFFF.CadastrBenin.DesktopApp.exe.config");

			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				Exception ie = e.InnerException;
				while (ie != null)
				{
					Console.WriteLine(ie.Message);
					ie = ie.InnerException;
				}

				Console.WriteLine("Сообщите информацию системному администратору.\nДля продолжения нажмите ENTER");
				Console.ReadLine();
			}
		}
		/// <summary>
		/// Включает поддержку CLR в экземпляре (localdb)\MSSQLLocalDB
		/// </summary>
		static void EnableDataBaseClr()
		{
			//Большая величина Connection Timeout обусловлена необходимостью первоначальной загрузки экземпляра localDB
			using (SqlConnection connection = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Connection Timeout=90"))
			{
				using (SqlCommand command = connection.CreateCommand())
				{
					connection.Open();

					command.CommandText = "EXEC sp_configure 'clr enabled', 1;";
					command.ExecuteNonQuery();

					command.CommandText = "RECONFIGURE;";
					command.ExecuteNonQuery();
				}
			}		
		}
		/// <summary>
		/// Прописывает путь к месту размещения файлов базы данных
		/// </summary>
		/// <param name="configFileName">Путь к конфигурационному файлу основного приложения</param>
		static void UpdateConnectionString(string configFileName)
		{
			string connectionString = @"metadata=res://*/CadastrBeninDBModel.csdl|res://*/CadastrBeninDBModel.ssdl|res://*/CadastrBeninDBModel.msl;provider=System.Data.SqlClient;provider connection string=""data source = (localdb)\MSSQLLocalDB;initial catalog = CadastrBenin;integrated security = True;AttachDbFileName = _DBPath_;MultipleActiveResultSets = True;App = EntityFramework""";


			string dbRelativePath = @"\db\SSDTCadastrBenin_Primary.mdf";
			string currentAppPath = System.IO.Path.GetDirectoryName(configFileName);
			connectionString = connectionString.Replace("_DBPath_", currentAppPath + dbRelativePath);


			XDocument configFile = XDocument.Load(configFileName);
			XElement elem = configFile.Element("configuration")
											.Element("connectionStrings")
												.Elements().Where(s => s.Attribute("name").Value == "CadastrBeninDB").Single();
			elem.Attribute("connectionString").Value = connectionString;

			configFile.Save(configFileName);
		}
	}
}
