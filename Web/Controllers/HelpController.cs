using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds)]
	public class HelpController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

	}
}
