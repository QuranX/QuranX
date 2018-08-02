using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Factories
{
	public class DocumentFactory
	{
		QuranDocument Quran;
		HadithDocument Hadith;
		TafsirDocument Tafsir;
		WordsDocument RootWords;
		CorpusDocument Corpus;
        LexiconDocument Lexicons;
		string GeneratedTranslationsDirectory;
		string GeneratedHadithsDirectory;
		string AdditionalHadithXRefsDirectory;
		string GeneratedTafsirsDirectory;
		string GeneratedCorpusXmlFilePath;
		string GeneratedLexiconsXmlDirectory;

		public Document Create(
			string generatedTranslationsDirectory,
			string generatedHadithsDirectory,
			string additionalHadithXRefsDirectory,
			string generatedTafsirsDirectory,
			string generatedCorpusXmlFilePath,
			string generatedLexiconsXmlDirectory
			)
		{
			this.GeneratedTranslationsDirectory = generatedTranslationsDirectory;
			this.GeneratedHadithsDirectory = generatedHadithsDirectory;
			this.AdditionalHadithXRefsDirectory = additionalHadithXRefsDirectory;
			this.GeneratedTafsirsDirectory = generatedTafsirsDirectory;
			this.GeneratedCorpusXmlFilePath = generatedCorpusXmlFilePath;
			this.GeneratedLexiconsXmlDirectory = generatedLexiconsXmlDirectory;

			CreateQuran();
			CreateHadith();
			CreateTafsir();
			CreateRootWords();
			CreateCorpus();
            CreateLexicons();
			return new Document(
					quranDocument: Quran,
					hadithDocument: Hadith,
					tafsirDocument: Tafsir,
					rootWordsDocument: RootWords,
					corpusDocument: Corpus,
                    lexiconDocument: null
				);
		}

		void CreateQuran()
		{
			Console.WriteLine("Loading Quran");
			var factory = new QuranDocumentFactory();
			Quran = factory.Create(GeneratedTranslationsDirectory);
		}

		void CreateHadith()
		{
			Console.WriteLine("Loading Hadiths");
			var factory = new HadithDocumentFactory();
			Hadith = factory.Create(
					generatedHadithsDirectory: GeneratedHadithsDirectory,
					additionalHadithXRefsDirectory: AdditionalHadithXRefsDirectory
				);
		}

		void CreateTafsir()
		{
			Console.WriteLine("Loading Tafsirs");
			var factory = new TafsirDocumentFactory();
			Tafsir = factory.Create(GeneratedTafsirsDirectory);
		}

		void CreateRootWords()
		{
			Console.WriteLine("Loading root words");
			var factory = new RootWordsDocumentFactory();
			RootWords = factory.Create(GeneratedCorpusXmlFilePath);
		}

		void CreateCorpus()
		{
			Console.WriteLine("Loading words by verse");
			var factory = new CorpusDocumentFactory();
			Corpus = factory.Create(GeneratedCorpusXmlFilePath);
		}

        void CreateLexicons()
        {
            Console.WriteLine("Creating lexicons");
            var factory = new LexiconDocumentFactory();
            Lexicons = factory.Create(GeneratedLexiconsXmlDirectory);
        }

	}
}
