using System.Collections.Generic;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class CommentariesForVerse
	{
		public readonly Chapter Chapter;
		public readonly int VerseNumber;
		public readonly IEnumerable<CommentatorAndCommentary> Commentaries;

		public CommentariesForVerse(
			Chapter chapter,
			int verseNumber,
			IEnumerable<CommentatorAndCommentary> commentaries)
		{
			Chapter = chapter;
			VerseNumber = verseNumber;
			Commentaries = commentaries;
		}
	}
}