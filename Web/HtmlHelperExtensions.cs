using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString Highlight(this HtmlHelper instance, string text)
		{
			string result = instance.Encode(text)
				.Replace("&lt;b&gt;", "<strong>")
				.Replace("&lt;/b&gt;", "</strong>");
			return MvcHtmlString.Create(result);
		}
	}
}