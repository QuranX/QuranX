using System.Collections.Generic;
using QuranX.Persistence.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Views.Commentary
{
	public class ViewModel
	{
		public Commentator Commentator { get; set; }
		public IEnumerable<ChapterAndVerseRangeReferenceSelection> Chapters { get; set; }

		public ViewModel(Commentator commentator, IEnumerable<ChapterAndVerseRangeReferenceSelection> chapters)
		{
			Commentator = commentator;
			Chapters = chapters;
		}
	}
}