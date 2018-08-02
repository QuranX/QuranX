using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds)]
	public class HomeController : Controller
	{
		public ActionResult About()
		{
			ViewBag.HideChapterVerseQuickJump = true;
			return View();
		}

	}
}
