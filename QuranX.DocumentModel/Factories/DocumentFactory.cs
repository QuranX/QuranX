using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Factories
{
	public class DocumentFactory
	{
		private readonly ILogger Logger;
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

		public DocumentFactory(ILogger logger)
		{
			Logger = logger;
		}

		public Document Create(
			string generatedTranslationsDirectory,
			string generatedHadithsDirectory,
			string additionalHadithXRefsDirectory,
			string generatedTafsirsDirectory,
			string generatedCorpusXmlFilePath,
			string generatedLexiconsXmlDirectory
			)
		{
			GeneratedTranslationsDirectory = generatedTranslationsDirectory;
			GeneratedHadithsDirectory = generatedHadithsDirectory;
			AdditionalHadithXRefsDirectory = additionalHadithXRefsDirectory;
			GeneratedTafsirsDirectory = generatedTafsirsDirectory;
			GeneratedCorpusXmlFilePath = generatedCorpusXmlFilePath;
			GeneratedLexiconsXmlDirectory = generatedLexiconsXmlDirectory;

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
			Logger.Debug("Loading Quran");
			var factory = new QuranDocumentFactory();
			Quran = factory.Create(GeneratedTranslationsDirectory);
		}

		void CreateHadith()
		{
			Logger.Debug("Loading Hadiths");
			var factory = new HadithDocumentFactory();
			Hadith = factory.Create(
					generatedHadithsDirectory: GeneratedHadithsDirectory,
					additionalHadithXRefsDirectory: AdditionalHadithXRefsDirectory
				);
		}

		void CreateTafsir()
		{
			Logger.Debug("Loading Tafsirs");
			var factory = new TafsirDocumentFactory();
			Tafsir = factory.Create(GeneratedTafsirsDirectory);
		}

		void CreateRootWords()
		{
			Logger.Debug("Loading Roots");
			var factory = new RootWordsDocumentFactory();
			RootWords = factory.Create(GeneratedCorpusXmlFilePath);
		}

		void CreateCorpus()
		{
			Logger.Debug("Loading Corpus");
			var factory = new CorpusDocumentFactory();
			Corpus = factory.Create(GeneratedCorpusXmlFilePath);
		}

		void CreateLexicons()
		{
			Logger.Debug("Loading Lexicons");
			var factory = new LexiconDocumentFactory();
			Lexicons = factory.Create(GeneratedLexiconsXmlDirectory);
		}

	}
}
