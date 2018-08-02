using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace System.Web.Mvc
{
	public class MovedPermanentlyResponse : ActionResult
	{
		string NewUrl;

		public MovedPermanentlyResponse(string newUrl)
		{
			this.NewUrl = newUrl;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.AddHeader("Location", NewUrl);
			context.HttpContext.Response.StatusCode = 301;
			context.HttpContext.Response.End();
		}
	}
}
