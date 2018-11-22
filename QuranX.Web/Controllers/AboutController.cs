using System.Web.Mvc;

namespace QuranX.Web.Controllers
{
	public class AboutController : Controller
	{
		public ActionResult Index()
		{
			return View("About");
		}
	}
}