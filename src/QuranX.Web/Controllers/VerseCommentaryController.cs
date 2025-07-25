﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Factories;
using QuranX.Web.Models;
using QuranX.Web.Views.VerseCommentary;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class VerseCommentaryController : Controller
	{
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly ICommentaryRepository CommentaryRepository;
		private readonly ICommentariesForVerseFactory CommentariesForVerseFactory;
		private readonly ISelectChapterAndVerseFactory SelectChapterAndVerseFactory;

		public VerseCommentaryController(
			ICommentatorRepository commentatorRepository,
			ICommentaryRepository commentaryRepository,
			ICommentariesForVerseFactory commentariesForVerseFactory,
			ISelectChapterAndVerseFactory selectChapterAndVerseFactory)
		{
			CommentatorRepository = commentatorRepository;
			CommentaryRepository = commentaryRepository;
			CommentariesForVerseFactory = commentariesForVerseFactory;
			SelectChapterAndVerseFactory = selectChapterAndVerseFactory;
		}

		public ActionResult Index(string commentatorCode, int chapterNumber, int verseNumber)
		{

			if (!CommentatorRepository.TryGet(commentatorCode, out Commentator commentator))
				return NotFound();

			Commentary commentary = CommentaryRepository.GetForVerse(
				commentatorCode: commentatorCode,
				chapterNumber: chapterNumber,
				verseNumber: verseNumber);
			if (commentary == null)
				commentary = new Commentary(
					commentatorCode: commentatorCode,
					chapterNumber: chapterNumber,
					firstVerseNumber: verseNumber,
					lastVerseNumber: verseNumber,
					text: new[] { new TextContent("No tafsir found for this verse", false) });

			var commentatorAndCommentary = new CommentatorAndCommentary(
				commentator: commentator,
				commentary: commentary);

			string url = $"/Tafsir/{commentatorCode}/";
			var selectChapterAndVerse = SelectChapterAndVerseFactory.CreateForCommentary(
				commentatorCode: commentatorCode,
				selectedChapterNumber: chapterNumber,
				selectedVerseNumber: verseNumber,
				url: url);

			var viewModel = new ViewModel(
				commentatorAndCommentary: commentatorAndCommentary,
				selectChapterAndVerse: selectChapterAndVerse);
			return View("VerseCommentary", viewModel);
		}
	}
}