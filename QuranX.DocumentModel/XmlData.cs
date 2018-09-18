using System.IO;

namespace QuranX.DocumentModel
{
	public class XmlData
	{
		public readonly QuranX.DocumentModel.Document Document;

		public XmlData(string appDataDirectory)
		{
			Document = CreateDocument(appDataDirectory);
		}

		private Document CreateDocument(string appDataDirectory)
		{
			var factory = new QuranX.DocumentModel.Factories.DocumentFactory();
			return factory.Create(
					generatedTranslationsDirectory: Path.Combine(appDataDirectory, "Translations"),
					generatedHadithsDirectory: Path.Combine(appDataDirectory, "Hadiths"),
					additionalHadithXRefsDirectory: Path.Combine(appDataDirectory, "HadithXRefs"),
					generatedTafsirsDirectory: Path.Combine(appDataDirectory, "Tafsirs"),
					generatedCorpusXmlFilePath: Path.Combine(appDataDirectory, "CorpusQuran.xml"),
					generatedLexiconsXmlDirectory: Path.Combine(appDataDirectory, "LaneLexicon/LaneLexicon.xml")
				);
		}
	}
}
