using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Factories
{
	public interface ICommentariesForVerseFactory
	{
		CommentariesForVerse Create(int chapterNumber, int verseNumber);
		CommentariesForVerse Create(string commentatorCode, int chapterNumber, int verseNumber);
	}

	public class CommentariesForVerseFactory : ICommentariesForVerseFactory
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly ICommentaryRepository CommentaryRepository;

		public CommentariesForVerseFactory(
			IChapterRepository chapterRepository,
			ICommentatorRepository commentatorRepository,
			ICommentaryRepository commentaryRepository)
		{
			ChapterRepository = chapterRepository;
			CommentatorRepository = commentatorRepository;
			CommentaryRepository = commentaryRepository;
		}

		public CommentariesForVerse Create(int chapterNumber, int verseNumber)
			=> Create(commentatorCode: null, chapterNumber: chapterNumber, verseNumber: verseNumber);

		public CommentariesForVerse Create(string commentatorCode, int chapterNumber, int verseNumber)
		{
			QuranStructure.EnsureChapterAndVerseAreValid(chapterNumber, verseNumber);

			Dictionary<string, Commentator> commentatorByCode =
				CommentatorRepository.GetAll()
				.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);

			Chapter chapter = ChapterRepository.Get(chapterNumber);
			IEnumerable<Commentary> commentaries;
			if (string.IsNullOrEmpty(commentatorCode))
				commentaries = CommentaryRepository.GetForVerse(chapterNumber, verseNumber);
			else
			{
				commentaries = new Commentary[]
				{
					CommentaryRepository.GetForVerse(commentatorCode, chapterNumber, verseNumber)
				}
				.Where(x => x != null);
			}

			IEnumerable<CommentatorAndCommentary> commentatorsAndCommentaries =
				commentaries
				.OrderBy(x => x.CommentatorCode)
				.Select(
					x => new CommentatorAndCommentary(
						commentator: commentatorByCode[x.CommentatorCode],
						commentary: x)
					);

			var selectChapterAndVerse = new SelectChapterAndVerse(
				selectedChapterNumber: chapterNumber,
				selectedVerseNumber: verseNumber,
				url: "/Tafsirs");
			var viewModel = new CommentariesForVerse(
				chapter: chapter,
				verseNumber: verseNumber,
				selectChapterAndVerse: selectChapterAndVerse,
				commentaries: commentatorsAndCommentaries);
			return viewModel;
		}
	}
}