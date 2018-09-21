using QuranX.DocumentModel;
using QuranX.Persistence.Services;
using QuranX.Persistence.Services.Repositories;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using QuranX.DataMigration.Services;
using VerseViewModel = QuranX.Persistence.Models.Verse;
using VerseTextViewModel = QuranX.Persistence.Models.VerseText;
using XmlDocument = QuranX.DocumentModel.Document;
using NLog;

namespace QuranX.DataMigration.Migrators
{
	public interface IQuranMigrator
	{
		void Migrate();
	}

	public class QuranMigrator : IQuranMigrator
	{
		private readonly XmlDocument XmlDocument;
		private readonly ISettings Settings;
		private readonly ILogger Logger;
		private readonly IVerseWriterRepository VerseWriterRepository;
		private readonly ILuceneIndexWriterProvider WriterProvider;

		public QuranMigrator(
			ISettings settings,
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			IVerseWriterRepository verseWriterRepository,
			ILuceneIndexWriterProvider luceneIndexWriterProvider)
		{
			Settings = settings;
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			VerseWriterRepository = verseWriterRepository;
			WriterProvider = luceneIndexWriterProvider;
		}

		public void Migrate()
		{
			Directory.Delete(Settings.DataPath, true);
			Directory.CreateDirectory(Settings.DataPath);
			WriteVerses();
			WriterProvider.GetIndexWriter().Commit();
			WriterProvider.GetIndexWriter().Optimize(true);
		}

		private void WriteVerses()
		{
			foreach (Chapter chapter in XmlDocument.QuranDocument.Chapters)
			{
				foreach (Verse verse in chapter.Verses)
				{
					WriteVerse(chapter, verse);
				}
			}
		}

		private void WriteVerse(Chapter chapter, Verse verse)
		{
			Logger.Debug($"Writing verse {chapter.Index}.{verse.Index}");
			var verseTexts = new List<VerseTextViewModel>();
			verseTexts.Add(new VerseTextViewModel("AR", "Arabic", verse.ArabicText));
			foreach (VerseTranslation translation in verse.Translations.OrderBy(x => x.TranslatorName))
			{
				verseTexts.Add(new VerseTextViewModel(translation.TranslatorCode, translation.TranslatorCode, translation.Text));
			}

			var verseViewModel = new VerseViewModel(
				chapterNumber: chapter.Index,
				verseNumber: verse.Index,
				rootWordCount: XmlDocument.CorpusDocument[chapter.Index, verse.Index].Words.Count(),
				hadithCount: XmlDocument.HadithDocument.GetHadithsForVerse(chapter.Index, verse.Index).Count(),
				commentaryCount: XmlDocument.TafsirDocument.GetCommentaries(chapter.Index, verse.Index).Count(),
				verseTexts: verseTexts);
			VerseWriterRepository.Write(verseViewModel);
		}
	}
}
