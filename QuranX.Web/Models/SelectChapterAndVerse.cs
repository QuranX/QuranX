using System.Collections.Generic;

namespace QuranX.Web.Models
{
	public class SelectChapterAndVerse
	{
		public readonly int SelectedChapterNumber;
		public readonly int SelectedVerseNumber;
		public readonly bool AllVerses;
		public readonly IEnumerable<ChapterAndVerseReferenceSelection> AvailableChapters;
		public readonly string Url;

		public SelectChapterAndVerse(
			int selectedChapterNumber,
			int selectedVerseNumber,
			bool allVerses,
			IEnumerable<ChapterAndVerseReferenceSelection> availableChapters,
			string url)
		{
			SelectedChapterNumber = selectedChapterNumber;
			SelectedVerseNumber = selectedVerseNumber;
			AllVerses = allVerses;
			AvailableChapters = availableChapters;
			if (!url.EndsWith("/"))
				url += "/";
			Url = url;
		}

		public SelectChapterAndVerse WithValues(
			int selectedChapterNumber, 
			int selectedVerseNumber,
			bool allVerses,
			string url)
		{
			return new SelectChapterAndVerse(
				selectedChapterNumber: selectedChapterNumber,
				selectedVerseNumber: selectedVerseNumber,
				allVerses: allVerses,
				availableChapters: AvailableChapters,
				url: url);
		}
	}
}