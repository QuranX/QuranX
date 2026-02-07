using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class ChapterAndVerseReferenceSelection
{
    public readonly Chapter Chapter;
    public readonly IEnumerable<VerseReference> VerseReferences;

    public ChapterAndVerseReferenceSelection(Chapter chapter, IEnumerable<VerseReference> verseReferences)
    {
        Chapter = chapter;
        VerseReferences = verseReferences;
    }
}