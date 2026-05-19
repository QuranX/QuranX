using QuranX.Shared;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories;

public static class ArabicWordIndexer
{
    public static string GetIndexForArabic(string word)
        => ArabicHelper.ArabicToLetterNames(ArabicHelper.SubstituteAndOmit(word)).Replace('-', 'x');
    public static IEnumerable<string> GetIndexForArabic(IEnumerable<string> words)
        => words.Select(GetIndexForArabic);
}
