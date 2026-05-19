using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.Shared;

public static class ArabicHelper
{
    const string ArabicAlphabet =
        "إ أ آ ا ب ت ة ث ج ح خ د ذ ر ز س ش ص ض ط ظ ع غ ف ق ك ل م ن ه و ي ؤ ء ة ئ ى ي ئ";

    static readonly HashSet<char> PermittedChars =
        new(ArabicAlphabet.ToCharArray().Distinct());
    public static readonly Dictionary<char, string> LetterNames = BuildLetterNames();
    public static readonly Dictionary<string, char> LettersByName =
        LetterNames.ToDictionary(x => x.Value, x => x.Key, StringComparer.OrdinalIgnoreCase);
    public static readonly Dictionary<char, char> AlternateChars = BuildAlternateChars();

    public static string ArabicToLetterNames(string source)
    {
        source = SubstituteAndOmit(source);
        bool isFirst = true;
        var builder = new StringBuilder();
        foreach (char arabicChar in source)
        {
            if (!LetterNames.TryGetValue(arabicChar, out string? letterName))
            {
                throw new ArgumentException($"No letter name for: {arabicChar}");
            }
            if (string.IsNullOrEmpty(letterName))
                continue;
            if (!isFirst)
                builder.Append('-');
            isFirst = false;
            builder.Append(letterName);
        }
        return builder.ToString();
    }

    public static string? LetterNamesToArabic(string source)
    {
        var builder = new StringBuilder();
        string[] parts = source.Split('-');
        foreach (string letterName in parts)
        {
            if (!LettersByName.TryGetValue(letterName, out char arabicLetter))
                return null;
            builder.Append(arabicLetter);
        }
        return builder.ToString();
    }

    public static string SubstituteAndOmit(string arabicText)
    {
        if (string.IsNullOrEmpty(arabicText))
            return arabicText;

        var resultBuilder = new StringBuilder();
        foreach (char c in arabicText.ToCharArray())
        {
            char alternateChar = GetArabicSubstituteChar(c);
            if (PermittedChars.Contains(alternateChar) && alternateChar != '\0')
                resultBuilder.Append(alternateChar);
        }
        return resultBuilder.ToString();
    }

    public static string Substitute(string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;
        var resultBuilder = new StringBuilder();
        foreach (char c in source)
            resultBuilder.Append(GetArabicSubstituteChar(c));
        return resultBuilder.ToString();
    }

    static Dictionary<char, char> BuildAlternateChars() => new()
    {
        ['ٱ'] = 'ا',
        ['آ'] = 'ا',
        ['أ'] = 'ا',
        ['ة'] = 'ت',
        ['ى'] = 'ي',
        ['ئ'] = 'ي',
        ['ء'] = '\0',
    };

    static char GetArabicSubstituteChar(char c) =>
        AlternateChars.TryGetValue(c, out char alternateChar) ? alternateChar : c;

    static Dictionary<char, string> BuildLetterNames() => new()
    {
        ['ا'] = "alif",
        ['ب'] = "ba",
        ['ت'] = "ta",
        ['ث'] = "tha",
        ['ج'] = "jim",
        ['ح'] = "ha",
        ['خ'] = "kha",
        ['د'] = "dal",
        ['ذ'] = "thal",
        ['ر'] = "ra",
        ['ز'] = "zay",
        ['س'] = "sin",
        ['ش'] = "shin",
        ['ص'] = "sad",
        ['ض'] = "dad",
        ['ط'] = "tta",
        ['ظ'] = "dha",
        ['ع'] = "ayn",
        ['غ'] = "ghayn",
        ['ف'] = "fa",
        ['ق'] = "qaf",
        ['ك'] = "kaf",
        ['ل'] = "lam",
        ['م'] = "mim",
        ['ن'] = "nun",
        ['ه'] = "heh",
        ['و'] = "waw",
        ['ي'] = "ya",
        ['ء'] = "",
    };
}
