using System.Web.Mvc;

namespace QuranX.Web.Controllers
{
	public class RedirectController : Controller
	{
		public ActionResult Index(string url)
		{
			return Redirect(url);
		}
	}
}