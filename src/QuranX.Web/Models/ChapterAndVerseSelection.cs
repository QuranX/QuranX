using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class ChapterAndVerseSelection
{
    public ChapterData Chapter { get; }
    public IEnumerable<Verse> Verses { get; }

    public ChapterAndVerseSelection(ChapterData chapter, IEnumerable<Verse> verses)
    {
        Chapter = chapter;
        Verses = verses;
    }
}