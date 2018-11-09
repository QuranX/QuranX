namespace QuranX.Persistence.Models
{
	public class VerseText
	{
		public string TranslatorCode { get; }
		public string TranslatorName { get; }
		public string Text { get; }

		public VerseText(string translatorCode, string translatorName, string text)
		{
			TranslatorCode = translatorCode;
			TranslatorName = translatorName;
			Text = text;
		}
	}
}
