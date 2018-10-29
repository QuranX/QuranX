using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseAnalysisWordRepository
	{
		IEnumerable<VerseAnalysisWord> GetForVerse(int chapterNumber, int verseNumber);
	}

	public class VerseAnalysisWordRepository : IVerseAnalysisWordRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public VerseAnalysisWordRepository(
			ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public IEnumerable<VerseAnalysisWord> GetForVerse(int chapterNumber, int verseNumber)
		{
			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<VerseAnalysisWord>()
				.AddNumericRangeQuery<VerseAnalysisWord>(
					x => x.ChapterNumber,
					lowerInclusive: chapterNumber,
					upperInclusive: chapterNumber,
					occur: Occur.MUST)
				.AddNumericRangeQuery<VerseAnalysisWord>(
					x => x.VerseNumber,
					lowerInclusive: verseNumber,
					upperInclusive: verseNumber,
					occur: Occur.MUST);

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = indexSearcher.Search(query, 999);
			IEnumerable<VerseAnalysisWord> result = docs .ScoreDocs
				.Select(x => indexSearcher.Doc(x.Doc))
				.Select(x => x.GetObject<VerseAnalysisWord>())
				.OrderBy(x => x.WordNumber);
			return result;
		}
	}
}
