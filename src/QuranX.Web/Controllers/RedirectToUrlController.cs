﻿using Microsoft.AspNetCore.Mvc;

namespace QuranX.Web.Controllers
{
	public class RedirectToUrlController : Controller
	{
		public ActionResult Index(string url)
		{
			return Redirect(url);
		}
	}
}