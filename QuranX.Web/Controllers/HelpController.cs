using System.Web.Mvc;

namespace QuranX.Web.Controllers
{
	public class HelpController : Controller
	{
		public ActionResult Index()
		{
			return View("Help");
		}
	}
}