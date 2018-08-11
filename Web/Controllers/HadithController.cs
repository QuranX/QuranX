using QuranX.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds)]
	public class HadithController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.HideChapterVerseQuickJump = true;
			var model = SharedData.Document.HadithDocument.Collections;
			return View(model);
		}

		public ActionResult Collection(string collectionCode, string indexCode, string path)
		{
			try
			{
				var model = new Hadith_DrillDown(
					collectionCode: collectionCode,
					indexCode: indexCode,
					path: path
				);
				return View(model);
			}
			catch (KeyNotFoundException)
			{
				return HttpNotFound();
			}
		}

		[HttpPost]
		public ActionResult RedirectToChapterVerse(int chapter, int verse)
		{
			return RedirectToAction(
				actionName: "Verse",
				controllerName: "HadithsByVerse",
				routeValues: new
				{
					Chapter = chapter,
					Verse = verse
				}
			);
		}



	}
}
