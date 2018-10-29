using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseAnalysisWordWriteRepository
	{
		void Write(VerseAnalysisWord verseAnalysisWord);
	}

	public class VerseAnalysisWordWriteRepository : IVerseAnalysisWordWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public VerseAnalysisWordWriteRepository(
			ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(VerseAnalysisWord verseAnalysisWord)
		{
			var document = new Document();
			document.StoreAndIndex(verseAnalysisWord, x => x.ChapterNumber);
			document.StoreAndIndex(verseAnalysisWord, x => x.VerseNumber);
			document.Index(verseAnalysisWord, x => x.Arabic);
			document.AddObject(verseAnalysisWord);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);

			WriteWordParts(indexWriter, verseAnalysisWord);
		}

		private static void WriteWordParts(
			IndexWriter indexWriter,
			VerseAnalysisWord verseAnalysisWord)
		{
			foreach (VerseAnalysisWordPart wordPart in verseAnalysisWord.WordParts)
			{
				var document = new Document();
				document.StoreAndIndex(verseAnalysisWord, x => x.ChapterNumber);
				document.StoreAndIndex(verseAnalysisWord, x => x.VerseNumber);
				document.StoreAndIndex(verseAnalysisWord, x => x.WordNumber);
				if (!string.IsNullOrEmpty(wordPart.Root))
					document.Index(wordPart, x => x.Root);

				indexWriter.AddDocument(document);
			}
		}
	}
}
