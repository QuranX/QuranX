using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel;

public class Hadith
{
    public readonly HadithCollection Collection;
    public readonly HadithReference PrimaryReference;
    public readonly HadithReference[] References;
    public readonly string[] ArabicText;
    public readonly string[] EnglishText;
    public readonly VerseRangeReference[] VerseReferences;
    Dictionary<string, HadithReference> ReferencesByCode;

    public Hadith(
    HadithCollection collection,
    IEnumerable<HadithReference> references,
    IEnumerable<string> arabicText,
        IEnumerable<string> englishText,
        IEnumerable<VerseRangeReference> verseReferences)
    {
        if (collection is null)
            throw new ArgumentNullException(nameof(collection));
        if (references is null || !references.Any())
            throw new ArgumentNullException(nameof(references));
        if (arabicText is null)
            throw new ArgumentNullException(nameof(arabicText));
        if (englishText is null)
            throw new ArgumentNullException(nameof(englishText));
        if (verseReferences is null)
            throw new ArgumentNullException(nameof(verseReferences));

        this.Collection = collection;
        this.References = references.ToArray();
        this.ArabicText = arabicText.ToArray();
        this.EnglishText = englishText.ToArray();
        this.VerseReferences = verseReferences.Distinct().OrderBy(x => x).ToArray();
        ReferencesByCode = references.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
        PrimaryReference = GetReference(collection.PrimaryReferenceDefinition.Code) ?? References.First();
    }

    public HadithReference GetReference(string code)
    {
        HadithReference result;
        if (ReferencesByCode.TryGetValue(code, out result))
            return result;
        return null;
    }
}
