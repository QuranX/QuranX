using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentaryRepository
	{
		IEnumerable<Commentary> GetForVerse(int chapterNumber, int verseNumber);
		Commentary GetForVerse(string commentatorCode, int chapterNumber, int verseNumber);
		IEnumerable<VerseRangeReference> GetVerseRangeReferences(string commentatorCode);
	}

	public class CommentaryRepository : ICommentaryRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;
		private readonly ConcurrentDictionary<string, VerseRangeReference[]> VerseReferencesByCommentatorCode;

		public CommentaryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
			VerseReferencesByCommentatorCode = 
				new ConcurrentDictionary<string, VerseRangeReference[]>(StringComparer.InvariantCultureIgnoreCase);
		}

		public IEnumerable<Commentary> GetForVerse(int chapterNumber, int verseNumber)
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
				.ThenBy(x => x.FirstVerseNumber);
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

		public IEnumerable<VerseRangeReference> GetVerseRangeReferences(string commentatorCode)
		{
			if (commentatorCode == null)
				throw new ArgumentNullException(nameof(commentatorCode));

			VerseRangeReference[] result;
			if (VerseReferencesByCommentatorCode.TryGetValue(commentatorCode, out result))
				return result;

			IndexSearcher indexSearcher = IndexSearcherProvider.GetIndexSearcher();
			IEnumerable<int> docIds =
				GetCommentaryIds(
					commentatorCode: commentatorCode,
					chapterNumber: null,
					verseNumber: null);
			result =
				docIds
				.Select(x => indexSearcher.Doc(x))
				.Select(x => new VerseRangeReference(
					chapter: x.GetStoredValue<Commentary>(i => i.ChapterNumber),
					firstVerse: x.GetStoredValue<Commentary>(i => i.FirstVerseNumber),
					lastVerse: x.GetStoredValue<Commentary>(i => i.LastVerseNumber)))
				.OrderBy(x => x)
				.ToArray();

			VerseReferencesByCommentatorCode[commentatorCode] = result;
			return result;
		}

		private IEnumerable<int> GetCommentaryIds(string commentatorCode, int? chapterNumber, int? verseNumber = null)
		{
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<Commentary>();

			if (commentatorCode != null)
				query.AddPhraseQuery<Commentary>(x => x.CommentatorCode, commentatorCode, Occur.MUST);

			if (chapterNumber != null)
			{
				query.AddNumericRangeQuery<Commentary>(
					x => x.ChapterNumber,
					lowerInclusive: chapterNumber.Value,
					upperInclusive: chapterNumber.Value,
					occur: Occur.MUST);

				if (verseNumber != null)
				{
					query
						.AddNumericRangeQuery<Commentary>(
							x => x.LastVerseNumber,
							lowerInclusive: 0,
							upperInclusive: verseNumber.Value - 1,
							occur: Occur.MUST_NOT)
						.AddNumericRangeQuery<Commentary>(
							x => x.FirstVerseNumber,
							lowerInclusive: verseNumber.Value + 1,
							upperInclusive: 999,
							occur: Occur.MUST_NOT);
				}
			}

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			IEnumerable<int> verses = docs.ScoreDocs.Select(x => x.Doc);
			return verses;
		}
	}
}
