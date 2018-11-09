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
		IEnumerable<VerseReference> GetVerseReferences();
		Verse GetVerse(VerseReference verseReference);
		IEnumerable<Verse> GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences);
	}

	public class VerseRepository : IVerseRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;
		private IEnumerable<VerseReference> AllReferences;

		public VerseRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public IEnumerable<VerseReference> GetVerseReferences()
		{
			if (AllReferences == null)
				AllReferences = QuranStructure.Chapters.Select(x => new VerseRangeReference(
					chapter: x.ChapterNumber,
					firstVerse: 1,
					lastVerse: x.NumberOfVerses))
					.SelectMany(x => x.ToVerseReferences());
			return AllReferences;
		}

		public IEnumerable<Verse> GetVerses(IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			IEnumerable<int> documentIds = verseRangeReferences.SelectMany(GetVerses).Distinct();

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			IEnumerable<Verse> verses = documentIds.Select(x => searcher.Doc(x).GetObject<Verse>());
			return verses;
		}

		public Verse GetVerse(VerseReference verseReference)
		{
			var verseRangeReference = new VerseRangeReference(
				chapter: verseReference.Chapter,
				firstVerse: verseReference.Verse,
				lastVerse: verseReference.Verse);
			return GetVerses(new VerseRangeReference[] { verseRangeReference }).Single();
		}

		private IEnumerable<int> GetVerses(VerseRangeReference verseRangeReference)
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
			IEnumerable<int> verses = docs.ScoreDocs.Select(x => x.Doc).ToArray();
			return verses;
		}
	}
}
