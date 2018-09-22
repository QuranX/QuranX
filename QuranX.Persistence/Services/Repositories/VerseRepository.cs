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
			IEnumerable<int> documentIds = verseRangeReferences.SelectMany(x => GetVerses(x)).Distinct();

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			Verse[] verses = documentIds.Select(x => searcher.Doc(x).GetObject<Verse>()).ToArray();
			return verses;
		}

		private int[] GetVerses(VerseRangeReference verseRangeReference)
		{
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<Verse>();

			var chapterQuery = NumericRangeQuery.NewIntRange(nameof(Verse.ChapterNumber), verseRangeReference.Chapter, verseRangeReference.Chapter, true, true);
			query.Add(chapterQuery, Occur.MUST);

			var verseQuery = NumericRangeQuery.NewIntRange(nameof(Verse.VerseNumber), verseRangeReference.FirstVerse, verseRangeReference.LastVerse, true, true);
			query.Add(verseQuery, Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			int[] verses = docs.ScoreDocs.Select(x => x.Doc).ToArray();

			return verses;
		}
	}
}
