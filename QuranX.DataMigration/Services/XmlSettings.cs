namespace QuranX.DataMigration.Services
{
	public interface IXmlSettings
	{
		string XmlDataPath { get;  }
	}

	public class XmlSettings : IXmlSettings
	{
		public string XmlDataPath { get; private set; }

		public XmlSettings(string xmlDataPath)
		{
			XmlDataPath = xmlDataPath;
		}
	}
}
