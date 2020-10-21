using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class Commentary
	{
		public string CommentatorCode { get; }
		public int ChapterNumber { get; }
		public int FirstVerseNumber { get; }
		public int LastVerseNumber { get; }
		public IReadOnlyList<TextContent> Text { get; }

		public Commentary(
			string commentatorCode,
			int chapterNumber,
			int firstVerseNumber,
			int lastVerseNumber,
			IEnumerable<TextContent> text)
		{
			CommentatorCode = commentatorCode;
			ChapterNumber = chapterNumber;
			FirstVerseNumber = firstVerseNumber;
			LastVerseNumber = lastVerseNumber;
			Text = text.ToList().AsReadOnly();
		}
	}
}
