using System.Web.Mvc;
using QuranX.Web.Builders;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class VerseCommentaryController : Controller
	{
		private readonly ICommentariesForVerseBuilder CommentariesForVerseBuilder;

		public VerseCommentaryController(ICommentariesForVerseBuilder commentariesForVerseBuilder)
		{
			CommentariesForVerseBuilder = commentariesForVerseBuilder;
		}

		// GET: VerseCommentary
		public ActionResult Index(string commentatorCode, int chapterNumber, int verseNumber)
		{
			CommentariesForVerse viewModel = CommentariesForVerseBuilder.Build(
				commentatorCode: commentatorCode,
				chapterNumber: chapterNumber,
				verseNumber: verseNumber);
			return View(viewModel);
		}
	}
}