using System.Collections.Generic;

namespace QuranX.Web.Models
{
	public class SelectChapterAndVerse
	{
		public readonly int SelectedChapterNumber;
		public readonly int SelectedVerseNumber;
		public readonly IEnumerable<ChapterAndVerseReferenceSelection> AvailableChapters;
		public readonly string Url;

		public SelectChapterAndVerse(
			int selectedChapterNumber,
			int selectedVerseNumber,
			IEnumerable<ChapterAndVerseReferenceSelection> availableChapters,
			string url)
		{
			SelectedChapterNumber = selectedChapterNumber;
			SelectedVerseNumber = selectedVerseNumber;
			AvailableChapters = availableChapters;
			if (!url.EndsWith("/"))
				url += "/";
			Url = url;
		}

		public SelectChapterAndVerse WithValues(int selectedChapterNumber, int selectedVerseNumber, string url)
		{
			return new SelectChapterAndVerse(
				selectedChapterNumber: selectedChapterNumber,
				selectedVerseNumber: selectedVerseNumber,
				availableChapters: AvailableChapters,
				url: url);
		}
	}
}