namespace QuranX.DataMigration.Services
{
	public interface IConfiguration
	{
		string XmlDataPath { get;  }
	}

	public class Configuration : IConfiguration
	{
		public string XmlDataPath { get; private set; }

		public Configuration(string xmlDataPath)
		{
			XmlDataPath = xmlDataPath;
		}
	}
}
