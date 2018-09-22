using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentaryRepository
	{
		Commentary[] GetForVerse(int chapterNumber, int verseNumber);
		Commentary GetForVerse(string commentatorCode, int chapterNumber, int verseNumber);
		VerseRangeReference[] GetVerseRangeReferences(string commentatorCode);
	}

	public class CommentaryRepository : ICommentaryRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public CommentaryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public Commentary[] GetForVerse(int chapterNumber, int verseNumber)
		{
			IEnumerable<int> docIds =
				GetCommentaryIds(
					commentatorCode: null,
					chapterNumber: chapterNumber,
					verseNumber: verseNumber)
				.Distinct();

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			return docIds
				.Select(x => indexSearcher.Doc(x).GetObject<Commentary>())
				.OrderBy(x => x.CommentatorCode)
				.ThenBy(x => x.ChapterNumber)
				.ThenBy(x => x.FirstVerseNumber)
				.ToArray();
		}

		public Commentary GetForVerse(string commentatorCode, int chapterNumber, int verseNumber)
		{
			if (commentatorCode == null)
				throw new ArgumentNullException(nameof(commentatorCode));

			IEnumerable<int> docIds =
				GetCommentaryIds(
					chapterNumber: chapterNumber,
					verseNumber: verseNumber,
					commentatorCode: commentatorCode)
				.Distinct();

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			return docIds
				.Select(x => indexSearcher.Doc(x).GetObject<Commentary>())
				.OrderBy(x => x.CommentatorCode)
				.ThenBy(x => x.ChapterNumber)
				.ThenBy(x => x.FirstVerseNumber)
				.Single();
		}

		public VerseRangeReference[] GetVerseRangeReferences(string commentatorCode)
		{
			if (commentatorCode == null)
				throw new ArgumentNullException(nameof(commentatorCode));

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			IEnumerable<int> docIds =
				GetCommentaryIds(
					commentatorCode: commentatorCode,
					chapterNumber: null,
					verseNumber: null);
			return
				docIds
				.Select(x => indexSearcher.Doc(x))
				.Select(x => new VerseRangeReference(
					chapter: int.Parse(x.GetField(nameof(Commentary.ChapterNumber)).StringValue),
					firstVerse: int.Parse(x.GetField(nameof(Commentary.FirstVerseNumber)).StringValue),
					lastVerse: int.Parse(x.GetField(nameof(Commentary.LastVerseNumber)).StringValue)))
				.OrderBy(x => x)
				.ToArray();
		}

		private int[] GetCommentaryIds(string commentatorCode, int? chapterNumber, int? verseNumber = null)
		{
			var query = new BooleanQuery(disableCoord: true);

			if (commentatorCode != null)
			{
				var codeTerm = new Term(nameof(Commentary.CommentatorCode), commentatorCode);
				var codeQuery = new TermQuery(codeTerm);
				query.Add(codeQuery, Occur.MUST);
			}

			if (chapterNumber != null)
			{
				var chapterQuery = NumericRangeQuery.NewIntRange(
					nameof(Verse.ChapterNumber),
					chapterNumber,
					chapterNumber,
					minInclusive: true,
					maxInclusive: true);
				query.Add(chapterQuery, Occur.MUST);

				if (verseNumber != null)
				{
					var excludeCommentariesFinishingBeforeRequiredVerseQuery = NumericRangeQuery.NewIntRange(
						nameof(Commentary.LastVerseNumber),
						0,
						verseNumber - 1,
						minInclusive: true,
						maxInclusive: true);
					query.Add(excludeCommentariesFinishingBeforeRequiredVerseQuery, Occur.MUST_NOT);

					var excludeCommentariesStartingAfterRequiredVerseQuery = NumericRangeQuery.NewIntRange(
						nameof(Commentary.FirstVerseNumber),
						verseNumber + 1,
						999,
						minInclusive: true,
						maxInclusive: true);
					query.Add(excludeCommentariesStartingAfterRequiredVerseQuery, Occur.MUST_NOT);
				}
			}

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			int[] verses = docs.ScoreDocs.Select(x => x.Doc).ToArray();
			return verses;
		}
	}
}
