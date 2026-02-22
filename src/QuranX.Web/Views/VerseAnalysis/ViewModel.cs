using QuranX.Shared.Models;
using QuranX.Web.Models;
using System;
using VerseAnalysisVM = QuranX.Persistence.Models.VerseAnalysis;

namespace QuranX.Web.Views.VerseAnalysis;

public class ViewModel
{
    public readonly ChapterData Chapter;
    public readonly int VerseNumber;
    public readonly VerseAnalysisVM VerseAnalysis;
    public readonly SelectChapterAndVerse SelectChapterAndVerse;

    public ViewModel(
        ChapterData chapter,
        int verseNumber,
        VerseAnalysisVM verseAnalysis,
        SelectChapterAndVerse selectChapterAndVerse)
    {
        Chapter = chapter;
        VerseNumber = verseNumber;
        VerseAnalysis = verseAnalysis ?? throw new ArgumentNullException(nameof(verseAnalysis));
        SelectChapterAndVerse = selectChapterAndVerse;
    }
}