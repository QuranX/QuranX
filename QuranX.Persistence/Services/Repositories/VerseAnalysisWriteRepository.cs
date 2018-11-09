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
			var document = new Document();
			document.StoreAndIndex(verseAnalysis, x => x.ChapterNumber);
			document.StoreAndIndex(verseAnalysis, x => x.VerseNumber);
			document.IndexArray(verseAnalysis, x => x.Roots);
			document.AddObject(verseAnalysis);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
