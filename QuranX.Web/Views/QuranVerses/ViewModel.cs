using System.Collections.Generic;
using QuranX.Web.Models;

namespace QuranX.Web.Views.QuranVerses
{
	public class ViewModel
	{
		public readonly IEnumerable<ChapterAndVerseSelection> DisplayVerses;
		public readonly SelectChapterAndVerse SelectChapterAndVerse;

		public ViewModel(IEnumerable<ChapterAndVerseSelection> displayVerses, SelectChapterAndVerse selectChapterAndVerse)
		{
			DisplayVerses = displayVerses;
			SelectChapterAndVerse = selectChapterAndVerse;
		}
	}
}