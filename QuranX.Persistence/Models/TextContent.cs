using System.Text.RegularExpressions;

namespace QuranX.Persistence.Models
{
	public class TextContent
	{
		public bool IsArabic { get; }
		public string Text { get; }

		public TextContent(string text, bool isArabic)
		{
			IsArabic = isArabic;
			Text = text;
		}

		public static TextContent Create(string text)
		{
			text = (text ?? "").ToLowerInvariant();
			return new TextContent(text, ContainsOnlyArabic(text));
		}

		public static bool ContainsOnlyArabic(string text)
		{
			if (!Regex.IsMatch(text, @"\p{IsArabic}"))
				return false;
			return !Regex.IsMatch(text, "[a-z]", RegexOptions.IgnoreCase);
		}

	}
}
