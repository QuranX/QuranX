using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class ChapterAndVerseSelection
{
    public readonly ChapterData Chapter;
    public readonly IEnumerable<Verse> Verses;

    public ChapterAndVerseSelection(ChapterData chapter, IEnumerable<Verse> verses)
    {
        Chapter = chapter;
        Verses = verses;
    }
}