using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranX.Web.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static string GetHexValues<TModel>(this IHtmlHelper<TModel> instance, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			var result = new StringBuilder();
			foreach (byte @byte in bytes)
				result.AppendFormat("{0:x2}", @byte);
			return result.ToString();
		}

		public static HtmlString Highlight<TModel>(this IHtmlHelper<TModel> instance, string text)
		{
			string result = instance.Encode(text)
				.Replace("&lt;b&gt;", "<strong>")
				.Replace("&lt;/b&gt;", "</strong>");
			return new HtmlString(result);
		}
	}
}