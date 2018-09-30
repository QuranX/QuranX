using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Factories
{
	public interface ISelectChapterAndVerseFactory
	{
		SelectChapterAndVerse CreateForAllChaptersAndVerses(
			int selectedChapterNumber,
			int selectedVerseNumber,
			string url);
		SelectChapterAndVerse CreateForCommentary(
			string commentatorCode,
			int selectedChapterNumber,
			int selectedVerseNumber,
			string url);
	}

	public class SelectChapterAndVerseFactory : ISelectChapterAndVerseFactory
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly IVerseRepository VerseRepository;
		private readonly ICommentaryRepository CommentaryRepository;
		private readonly ConcurrentDictionary<string, SelectChapterAndVerse> ResultByCommentatorCode;
		private SelectChapterAndVerse ResultForAllChapters;

		public SelectChapterAndVerseFactory(
			IChapterRepository chapterRepository,
			IVerseRepository verseRepository,
			ICommentaryRepository commentaryRepository)
		{
			ChapterRepository = chapterRepository;
			VerseRepository = verseRepository;
			CommentaryRepository = commentaryRepository;
			ResultByCommentatorCode =
				new ConcurrentDictionary<string, SelectChapterAndVerse>(StringComparer.InvariantCultureIgnoreCase);
		}

		public SelectChapterAndVerse CreateForAllChaptersAndVerses(
			int selectedChapterNumber, 
			int selectedVerseNumber, 
			string url)
		{
			if (ResultForAllChapters == null)
			{
				IEnumerable<ChapterAndVerseReferenceSelection> availableChapters =
					VerseRepository.GetVerseReferences()
					.GroupBy(x => x.Chapter)
					.Select(x => new ChapterAndVerseReferenceSelection(
						chapter: ChapterRepository.Get(x.Key),
						verseReferences: x));
				ResultForAllChapters = new SelectChapterAndVerse(
					selectedChapterNumber: 1,
					selectedVerseNumber: 1,
					allVerses: true,
					url: url,
					availableChapters: availableChapters);
			}
			return ResultForAllChapters.WithValues(
				selectedChapterNumber: selectedChapterNumber,
				selectedVerseNumber: selectedVerseNumber,
				allVerses: true,
				url: url);
		}

		public SelectChapterAndVerse CreateForCommentary(
			string commentatorCode,
			int selectedChapterNumber,
			int selectedVerseNumber,
			string url)
		{
			if (!ResultByCommentatorCode.TryGetValue(commentatorCode, out SelectChapterAndVerse result))
			{
				IEnumerable<VerseReference> verseRangeReferences =
					CommentaryRepository
					.GetVerseRangeReferences(commentatorCode)
					.Select(x => new VerseReference(x.Chapter, x.FirstVerse))
					.Distinct();

				IEnumerable<ChapterAndVerseReferenceSelection> availableChaptersAndVerses =
					verseRangeReferences
					.GroupBy(x => x.Chapter)
					.Select(x => new ChapterAndVerseReferenceSelection(
						chapter: ChapterRepository.Get(x.Key),
						verseReferences: x.OrderBy(v => v.Verse)));

				result = new SelectChapterAndVerse(
					selectedChapterNumber: 1,
					selectedVerseNumber: 1,
					allVerses: false,
					url: url,
					availableChapters: availableChaptersAndVerses);

				ResultByCommentatorCode[commentatorCode] = result;
			}

			return result.WithValues(
				selectedChapterNumber: selectedChapterNumber,
				selectedVerseNumber: selectedVerseNumber,
				allVerses: false,
				url: url);
		}
	}
}