using System.Web.Mvc;
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
			Commentator commentator = CommentatorRepository.Get(commentatorCode);
			Commentary commentary = CommentaryRepository.GetForVerse(
				commentatorCode: commentatorCode,
				chapterNumber: chapterNumber,
				verseNumber: verseNumber);
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
			return View(viewModel);
		}
	}
}