using System.IO;

namespace QuranX
{
    public static class SharedData
    {
        static object SyncRoot = new object();

		public static QuranX.DocumentModel.Document Document { get; private set; }


        public static void Initialize()
        {
            string appDataDirectory = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
            var factory = new QuranX.DocumentModel.Factories.DocumentFactory();
            Document = factory.Create(
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