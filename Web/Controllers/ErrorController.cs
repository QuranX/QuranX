using System.Web.Mvc;

namespace QuranX.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult NotFound()
		{
			return HttpNotFound();
		}
	}
}