using System.Collections.Generic;
using QuranX.Web.Models;

namespace QuranX.Web.Views.QuranVerses
{
	public class ViewModel
	{
		public readonly IEnumerable<ChapterAndVerseSelection> DisplayVerses;
		public readonly IEnumerable<ChapterAndVerseReferenceSelection> AllVerses;

		public ViewModel(IEnumerable<ChapterAndVerseSelection> displayVerses, IEnumerable<ChapterAndVerseReferenceSelection> allVerses)
		{
			DisplayVerses = displayVerses;
			AllVerses = allVerses;
		}
	}
}