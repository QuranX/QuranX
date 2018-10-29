using System.Text;
using System.Web.Mvc;

public static class HtmlHelperExtensions
{
	public static string GetHexValues(this HtmlHelper instance, string value)
	{
		byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(value);
		var result = new StringBuilder();
		foreach (byte @byte in bytes)
			result.AppendFormat("{0:x2}", @byte);
		return result.ToString();
	}
}
