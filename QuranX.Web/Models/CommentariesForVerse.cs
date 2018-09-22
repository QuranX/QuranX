using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class CommentariesForVerse
	{
		public readonly Chapter Chapter;
		public readonly int VerseNumber;
		public readonly CommentatorAndCommentary[] Commentaries;

		public CommentariesForVerse(
			Chapter chapter,
			int verseNumber,
			CommentatorAndCommentary[] commentaries)
		{
			Chapter = chapter;
			VerseNumber = verseNumber;
			Commentaries = commentaries;
		}
	}
}