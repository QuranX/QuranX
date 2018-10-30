using System.Web.Mvc;

namespace QuranX.Controllers
{
	public class RedirectController : Controller
	{
		public ActionResult Redirect(string newController, string newAction)
		{
			string newUrl = Url.Action(actionName: newAction, controllerName: newController);
			return new MovedPermanentlyResponse(newUrl);
		}
	}
}
