using QuranX.Web.Models;

namespace QuranX.Web.Views.VerseCommentary
{
	public class ViewModel
	{
		public readonly CommentatorAndCommentary CommentatorAndCommentary;
		public readonly SelectChapterAndVerse SelectChapterAndVerse;

		public ViewModel(
			CommentatorAndCommentary commentatorAndCommentary,
			SelectChapterAndVerse selectChapterAndVerse)
		{
			CommentatorAndCommentary = commentatorAndCommentary;
			SelectChapterAndVerse = selectChapterAndVerse;
		}
	}
}