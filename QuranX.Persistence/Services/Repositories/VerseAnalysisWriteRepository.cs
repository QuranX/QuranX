using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseAnalysisWriteRepository
	{
		void Write(VerseAnalysis verseAnalysis);
	}

	public class VerseAnalysisWriteRepository : IVerseAnalysisWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public VerseAnalysisWriteRepository(
			ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(VerseAnalysis verseAnalysis)
		{
			IEnumerable<string> roots = verseAnalysis.Words
				.SelectMany(x => x.WordParts)
				.Where(x => !string.IsNullOrWhiteSpace(x.Root))
				.Select(x => x.Root)
				.Distinct()
				.Select(ArabicWordIndexer.GetIndexForArabic);

			var document = new Document();
			document.StoreAndIndex(verseAnalysis, x => x.ChapterNumber);
			document.StoreAndIndex(verseAnalysis, x => x.VerseNumber);
			document.IndexArray(VerseAnalysisRepository.RootWordsIndexName, roots);
			document.AddObject(verseAnalysis);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
