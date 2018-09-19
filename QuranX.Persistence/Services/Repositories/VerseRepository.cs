using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseRepository
	{
		Task<Verse[]> GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences);
	}

	public class VerseRepository : IVerseRepository
	{
		private readonly ILuceneIndexSearcherProvider _indexSearcherProvider;

		public VerseRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			_indexSearcherProvider = indexSearcherProvider;
		}

		public async Task<Verse[]> GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			Task<Verse[]>[] tasks = verseRangeReferences.Select(x => GetVerses(x)).ToArray();
			await Task.WhenAll(tasks);
			return tasks.SelectMany(x => x.Result).ToArray();
		}

		private Task<Verse[]> GetVerses(VerseRangeReference verseRangeReference)
		{
			return Task.Run(() =>
			{
				var query = new BooleanQuery();

				var chapterQuery = NumericRangeQuery.NewIntRange(nameof(Verse.ChapterNumber), verseRangeReference.Chapter, verseRangeReference.Chapter, true, true);
				query.Add(chapterQuery, Occur.MUST);

				var verseQuery = NumericRangeQuery.NewIntRange(nameof(Verse.VerseNumber), verseRangeReference.FirstVerse, verseRangeReference.LastVerse, true, true);
				query.Add(verseQuery, Occur.MUST);

				IndexSearcher searcher = _indexSearcherProvider.GetIndexSearcher();
				TopDocs docs = searcher.Search(query, 7000);
				Verse[] verses = docs.ScoreDocs.Select(x => searcher.Doc(x.Doc).GetObject<Verse>()).ToArray();

				return verses;
			});
		}
	}
}
