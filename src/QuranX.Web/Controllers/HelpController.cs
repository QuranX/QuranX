using System.Web.Mvc;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class HelpController : Controller
	{
		public ActionResult Index()
		{
			return View("Help");
		}
	}
}