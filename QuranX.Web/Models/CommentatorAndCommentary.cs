using QuranX.Persistence.Models;

namespace QuranX.Web.Models
{
	public class CommentatorAndCommentary
	{
		public readonly Commentator Commentator;
		public readonly Commentary Commentary;

		public CommentatorAndCommentary(Commentator commentator, Commentary commentary)
		{
			Commentator = commentator;
			Commentary = commentary;
		}
	}
}