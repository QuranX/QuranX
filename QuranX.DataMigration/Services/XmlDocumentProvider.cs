using NLog;
using QuranX.DocumentModel;

namespace QuranX.DataMigration.Services
{
	public interface IXmlDocumentProvider
	{
		Document Document { get; }
	}

	public class XmlDocumentProvider : IXmlDocumentProvider
	{
		private readonly XmlData XmlData;
		public Document Document => XmlData.Document;

		public XmlDocumentProvider(IXmlSettings settings, ILogger logger)
		{
			logger.Debug("Loading XML data");
			XmlData = new XmlData(settings.XmlDataPath, logger);
		}


	}
}
