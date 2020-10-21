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

		public XmlDocumentProvider(IConfiguration configuration, ILogger logger)
		{
			logger.Debug("Loading XML data");
			XmlData = new XmlData(configuration.XmlDataPath, logger);
		}
	}
}
