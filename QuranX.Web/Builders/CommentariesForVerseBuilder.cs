using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Builders
{
	public interface ICommentariesForVerseBuilder
	{
		CommentariesForVerse Build(int chapterNumber, int verseNumber);
		CommentariesForVerse Build(string commentatorCode, int chapterNumber, int verseNumber);
	}

	public class CommentariesForVerseBuilder : ICommentariesForVerseBuilder
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly ICommentaryRepository CommentaryRepository;

		public CommentariesForVerseBuilder(
			IChapterRepository chapterRepository,
			ICommentatorRepository commentatorRepository,
			ICommentaryRepository commentaryRepository)
		{
			ChapterRepository = chapterRepository;
			CommentatorRepository = commentatorRepository;
			CommentaryRepository = commentaryRepository;
		}

		public CommentariesForVerse Build(int chapterNumber, int verseNumber)
			=> Build(commentatorCode: null, chapterNumber: chapterNumber, verseNumber: verseNumber);

		public CommentariesForVerse Build(string commentatorCode, int chapterNumber, int verseNumber)
		{
			QuranStructure.EnsureChapterAndVerseAreValid(chapterNumber, verseNumber);

			Dictionary<string, Commentator> commentatorByCode =
				CommentatorRepository.GetAll()
				.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);

			Chapter chapter = ChapterRepository.Get(chapterNumber);
			IEnumerable<Commentary> commentaries =
				string.IsNullOrEmpty(commentatorCode)
				? CommentaryRepository.GetForVerse(chapterNumber, verseNumber)
				: new Commentary[] { CommentaryRepository.GetForVerse(commentatorCode, chapterNumber, verseNumber) };

			IEnumerable<CommentatorAndCommentary> commentatorsAndCommentaries =
				commentaries
				.OrderBy(x => x.CommentatorCode)
				.Select(
					x => new CommentatorAndCommentary(
						commentator: commentatorByCode[x.CommentatorCode],
						commentary: x)
					);
			var viewModel = new CommentariesForVerse(
				chapter: chapter,
				verseNumber: verseNumber,
				commentaries: commentatorsAndCommentaries);
			return viewModel;
		}
	}
}