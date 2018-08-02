using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuranX.Controllers
{
    public class RedirectController : Controller
    {
        //
        // GET: /Redirect/

        public ActionResult Redirect(string newController, string newAction)
        {
			string newUrl = Url.Action(actionName: newAction, controllerName: newController);
			return new MovedPermanentlyResponse(newUrl);
        }

    }
}
