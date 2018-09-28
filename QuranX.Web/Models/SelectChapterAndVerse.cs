using System.Collections.Generic;

namespace QuranX.Web.Models
{
	public class SelectChapterAndVerse
	{
		public readonly int SelectedChapterNumber;
		public readonly int SelectedVerseNumber;
		public readonly IEnumerable<ChapterAndVerseReferenceSelection> AvailableChapters;

		public SelectChapterAndVerse(int selectedChapterNumber, int selectedVerseNumber, IEnumerable<ChapterAndVerseReferenceSelection> availableChapters)
		{
			SelectedChapterNumber = selectedChapterNumber;
			SelectedVerseNumber = selectedVerseNumber;
			AvailableChapters = availableChapters;
		}
	}
}