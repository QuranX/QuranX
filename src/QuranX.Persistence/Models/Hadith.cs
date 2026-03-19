using QuranX.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models;

public class Hadith
{
    public string CollectionCode { get; }
    public IReadOnlyList<string> ArabicText { get; }
    public IReadOnlyList<string> EnglishText { get; }
    public IReadOnlyList<VerseRangeReference> VerseRangeReferences { get; }
    public IReadOnlyList<HadithReference> References { get; }
    public string PrimaryReferenceCode { get; }
    public string PrimaryReferencePath { get; }

    public Hadith(
        string collectionCode,
        IEnumerable<string> arabicText,
        IEnumerable<string> englishText,
        IEnumerable<VerseRangeReference> verseRangeReferences,
        IEnumerable<HadithReference> references,
        string primaryReferenceCode,
        string primaryReferencePath)
    {
        CollectionCode = collectionCode;
        PrimaryReferenceCode = primaryReferenceCode;
        ArabicText = arabicText.ToList().AsReadOnly();
        EnglishText = englishText.ToList().AsReadOnly();
        VerseRangeReferences = verseRangeReferences.ToList().AsReadOnly();
        References = references.ToList().AsReadOnly();
        PrimaryReferencePath = primaryReferencePath;
    }
}
