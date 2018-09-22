namespace QuranX.Persistence.Models
{
	public class Commentary
	{
		public string CommentatorCode { get; set; }
		public int ChapterNumber { get; set; }
		public int FirstVerseNumber { get; set; }
		public int LastVerseNumber { get; set; }
		public string[] Text { get; }

		public Commentary(
			string commentatorCode,
			int chapterNumber,
			int firstVerseNumber,
			int lastVerseNumber,
			string[] text)
		{
			CommentatorCode = commentatorCode;
			ChapterNumber = chapterNumber;
			FirstVerseNumber = firstVerseNumber;
			LastVerseNumber = lastVerseNumber;
			Text = text;
		}
	}
}
