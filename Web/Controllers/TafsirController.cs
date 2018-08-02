using QuranX.Models;
using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds)]
	public class TafsirController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.HideChapterVerseQuickJump = true;
			var model = SharedData.Document.TafsirDocument.Tafsirs;
			return View(model);
		}

		public ActionResult CommentaryIndexes(string tafsir)
		{
			var model = SharedData.Document.TafsirDocument[tafsir];
			return View(model);
		}

		public ActionResult Verse(string tafsir, int chapter, int verse)
		{
			ViewBag.Chapter = chapter;
			ViewBag.Verse = verse;
			var model = new Tafsir_Commentary(
				tafsirCode: tafsir,
				chapter: chapter,
				verse: verse
			);
			return View(model);
		}

		[HttpPost]
		public ActionResult RedirectToChapterVerse(string tafsir, int chapter, int verse)
		{
			return RedirectToAction("Verse", new
			{
				Tafsir = tafsir,
				Chapter = chapter,
				Verse = verse
			});
		}


	}
}
