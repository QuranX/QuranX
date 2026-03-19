using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class ChapterAndVerseSelection
{
    public ChapterData Chapter { get; set; }
    public IEnumerable<Verse> Verses { get; set; }

    public ChapterAndVerseSelection(ChapterData chapter, IEnumerable<Verse> verses)
    {
        Chapter = chapter;
        Verses = verses;
    }
}