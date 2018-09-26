using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class HomeController : Controller
	{
		public ActionResult About()
		{
			ViewBag.HideChapterVerseQuickJump = true;
			return View();
		}

	}
}
