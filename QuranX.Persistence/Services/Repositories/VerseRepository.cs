using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseRepository
	{
		Verse[] GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences);
	}

	public class VerseRepository : IVerseRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public VerseRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public Verse[] GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			IEnumerable<int> documentIds = verseRangeReferences.SelectMany(GetVerses).Distinct();

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			Verse[] verses = documentIds.Select(x => searcher.Doc(x).GetObject<Verse>()).ToArray();
			return verses;
		}

		private int[] GetVerses(VerseRangeReference verseRangeReference)
		{
			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<Verse>()
				.AddNumericRangeQuery<Verse>(
					x => x.ChapterNumber,
					lowerInclusive: verseRangeReference.Chapter,
					upperInclusive: verseRangeReference.Chapter,
					occur: Occur.MUST)
				.AddNumericRangeQuery<Verse>(
					x => x.VerseNumber,
					lowerInclusive: verseRangeReference.FirstVerse,
					upperInclusive: verseRangeReference.LastVerse,
					occur: Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			int[] verses = docs.ScoreDocs.Select(x => x.Doc).ToArray();

			return verses;
		}
	}
}
