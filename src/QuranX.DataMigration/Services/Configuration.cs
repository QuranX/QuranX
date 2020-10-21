using System.IO;

namespace QuranX.DataMigration.Services
{
	public interface IConfiguration
	{
		string XmlDataPath { get;  }
		string DictionariesDirectoryPath { get; }
	}

	public class Configuration : IConfiguration
	{
		public string XmlDataPath { get; }
		public string DictionariesDirectoryPath { get; }

		public Configuration(string appDataPath)
		{
			XmlDataPath = appDataPath;
			DictionariesDirectoryPath = Path.Combine(appDataPath, "Dictionaries");
		}
	}
}
