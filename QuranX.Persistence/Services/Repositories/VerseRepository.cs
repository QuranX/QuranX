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
		private readonly ILuceneIndexSearcherProvider _indexSearcherProvider;

		public VerseRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			_indexSearcherProvider = indexSearcherProvider;
		}

		public Verse[] GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			IndexSearcher searcher = _indexSearcherProvider.GetIndexSearcher();
			int[] documentIds = verseRangeReferences.SelectMany(x => GetVerses(x)).Distinct().ToArray();
			Verse[] verses = documentIds.Select(x => searcher.Doc(x).GetObject<Verse>()).ToArray();
			return verses;
		}

		private int[] GetVerses(VerseRangeReference verseRangeReference)
		{
			var query = new BooleanQuery();

			var chapterQuery = NumericRangeQuery.NewIntRange(nameof(Verse.ChapterNumber), verseRangeReference.Chapter, verseRangeReference.Chapter, true, true);
			query.Add(chapterQuery, Occur.MUST);

			var verseQuery = NumericRangeQuery.NewIntRange(nameof(Verse.VerseNumber), verseRangeReference.FirstVerse, verseRangeReference.LastVerse, true, true);
			query.Add(verseQuery, Occur.MUST);

			IndexSearcher searcher = _indexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			int[] verses = docs.ScoreDocs.Select(x => x.Doc).ToArray();

			return verses;
		}
	}
}
