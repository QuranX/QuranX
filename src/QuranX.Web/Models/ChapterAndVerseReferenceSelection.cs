using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class ChapterAndVerseReferenceSelection
{
    public readonly ChapterData Chapter;
    public readonly IEnumerable<VerseReference> VerseReferences;

    public ChapterAndVerseReferenceSelection(ChapterData chapter, IEnumerable<VerseReference> verseReferences)
    {
        Chapter = chapter;
        VerseReferences = verseReferences;
    }
}