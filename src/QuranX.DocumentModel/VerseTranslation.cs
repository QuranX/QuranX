namespace QuranX.DocumentModel
{
	public class VerseTranslation
	{
		public readonly string TranslatorCode;
		public readonly string TranslatorName;
		public readonly string Text;

		public VerseTranslation(
			string translatorCode, 
			string translatorName, 
			string text)
		{
			this.TranslatorCode = translatorCode;
			this.TranslatorName = translatorName;
			this.Text = text;
		}
	}
}
