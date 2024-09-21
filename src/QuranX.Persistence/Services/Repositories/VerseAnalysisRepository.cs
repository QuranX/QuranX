using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseAnalysisRepository
	{
		VerseAnalysis GetForVerse(int chapterNumber, int verseNumber);
		IEnumerable<VerseAnalysis> GetForRoot(string root);
	}

	public class VerseAnalysisRepository : IVerseAnalysisRepository
	{
		public const string RootWordsIndexName = nameof(VerseAnalysis) + "_RootsIndex";
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public VerseAnalysisRepository(
			ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public VerseAnalysis GetForVerse(int chapterNumber, int verseNumber)
		{
			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<VerseAnalysis>()
				.AddNumericRangeQuery<VerseAnalysis>(
					x => x.ChapterNumber,
					lowerInclusive: chapterNumber,
					upperInclusive: chapterNumber,
					occur: Occur.MUST)
				.AddNumericRangeQuery<VerseAnalysis>(
					x => x.VerseNumber,
					lowerInclusive: verseNumber,
					upperInclusive: verseNumber,
					occur: Occur.MUST);

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = indexSearcher.Search(query, 999);
			VerseAnalysis result = docs.ScoreDocs
				.Select(x => indexSearcher.Doc(x.Doc))
				.Single()
				.GetObject<VerseAnalysis>();
			return result;
		}

		public IEnumerable<VerseAnalysis> GetForRoot(string root)
		{
			string rootIndex = ArabicWordIndexer.GetIndexForArabic(root);
			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<VerseAnalysis>()
				.AddStringEqualsQuery(RootWordsIndexName, rootIndex, Occur.MUST);

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs topDocs = indexSearcher.Search(query, 9999);

			IEnumerable<VerseAnalysis> result = topDocs.ScoreDocs
				.Select(x => indexSearcher.Doc(x.Doc))
				.Select(x => x.GetObject<VerseAnalysis>());
			return result;
		}
	}
}
