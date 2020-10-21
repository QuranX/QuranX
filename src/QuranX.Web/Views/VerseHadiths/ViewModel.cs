using System.Collections.Generic;
using QuranX.Shared.Models;
using QuranX.Web.Views.Shared;

namespace QuranX.Web.Views.VerseHadiths
{
	public class ViewModel
	{
		public readonly Chapter Chapter;
		public readonly int VerseNumber;
		public readonly IEnumerable<HadithViewModel> Hadiths;

		public ViewModel(Chapter chapter, int verseNumber, IEnumerable<HadithViewModel> hadiths)
		{
			Chapter = chapter;
			VerseNumber = verseNumber;
			Hadiths = hadiths;
		}
	}
}