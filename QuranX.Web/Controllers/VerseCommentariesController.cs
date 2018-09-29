using System.Web.Mvc;
using QuranX.Web.Factories;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class VerseCommentariesController : Controller
	{
		private readonly ICommentariesForVerseFactory CommentariesForVerseBuilder;

		public VerseCommentariesController(ICommentariesForVerseFactory commentariesForVerseBuilder)
		{
			CommentariesForVerseBuilder = commentariesForVerseBuilder;
		}

		// GET: VerseCommentary
		public ActionResult Index(string commentatorCode, int chapterNumber, int verseNumber)
		{
			CommentariesForVerse viewModel = CommentariesForVerseBuilder.Create(
				commentatorCode: commentatorCode,
				chapterNumber: chapterNumber,
				verseNumber: verseNumber);
			return View(viewModel);
		}
	}
}