using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Shared.Models;
using QuranX.Web.Factories;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class CommentariesForVerseController : Controller
	{
		private readonly ICommentariesForVerseFactory CommentariesForVerseBuilder;

		public CommentariesForVerseController(ICommentariesForVerseFactory commentariesForVerseBuilder)
		{
			CommentariesForVerseBuilder = commentariesForVerseBuilder;
		}

		public ActionResult Index(string commentatorCode, int chapterNumber, int verseNumber)
		{
			if (!QuranStructure.TryValidateChapterAndVerse(chapterNumber, verseNumber))
				return NotFound();

			CommentariesForVerse viewModel = CommentariesForVerseBuilder.Create(
				commentatorCode: commentatorCode,
				chapterNumber: chapterNumber,
				verseNumber: verseNumber);
			return View("CommentariesForVerse", viewModel);
		}
	}
}