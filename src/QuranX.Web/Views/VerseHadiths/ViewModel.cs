using QuranX.Shared.Models;
using QuranX.Web.Views.Shared;
using System.Collections.Generic;

namespace QuranX.Web.Views.VerseHadiths;

public class ViewModel
{
    public readonly ChapterData Chapter;
    public readonly int VerseNumber;
    public readonly IEnumerable<HadithViewModel> Hadiths;

    public ViewModel(ChapterData chapter, int verseNumber, IEnumerable<HadithViewModel> hadiths)
    {
        Chapter = chapter;
        VerseNumber = verseNumber;
        Hadiths = hadiths;
    }
}