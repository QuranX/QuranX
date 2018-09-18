namespace QuranX.Persistence.Models
{
	public class VerseText
	{
		public string TranslatorCode { get; set; }
		public string TranslatorName { get; set; }
		public string Text { get; set; }

		public VerseText() { }

		public VerseText(string translatorCode, string translatorName, string text)
		{
			TranslatorCode = translatorCode;
			TranslatorName = translatorName;
			Text = text;
		}
	}
}
