using System.IO;
using NLog;

namespace QuranX.DocumentModel
{
	public class XmlData
	{
		public readonly Document Document;

		public XmlData(string dataDirectory, ILogger logger)
		{
			Document = CreateDocument(dataDirectory, logger);
		}

		private Document CreateDocument(string dataDirectory, ILogger logger)
		{
			var factory = new QuranX.DocumentModel.Factories.DocumentFactory(logger);
			return factory.Create(
				generatedTranslationsDirectory: Path.Combine(dataDirectory, "Translations"),
				generatedHadithsDirectory: Path.Combine(dataDirectory, "Hadiths"),
				additionalHadithXRefsDirectory: Path.Combine(dataDirectory, "HadithXRefs"),
				generatedTafsirsDirectory: Path.Combine(dataDirectory, "Tafsirs"),
				generatedCorpusXmlFilePath: Path.Combine(dataDirectory, "CorpusQuran.xml"),
				generatedLexiconsXmlDirectory: Path.Combine(dataDirectory, "LaneLexicon/LaneLexicon.xml"));
		}
	}
}
