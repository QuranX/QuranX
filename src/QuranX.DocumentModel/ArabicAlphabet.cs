using System;
using System.Collections.Generic;
using System.Text;

namespace QuranX.DocumentModel;

public static class ArabicAlphabet
{
    const string AllLetters =
        "إ أ آ ا ب ت ث ج ح خ د ذ ر ز س ش ص ض ط ظ ع غ ف ق ك ل م ن ه و ي ؤ ء ة ئ ى ";

    static readonly Dictionary<char, string> Letters = BuildLetters();

    public static string GetSimplifiedArabicChar(char originalArabicChar)
    {
        if (!Letters.TryGetValue(originalArabicChar, out string result))
            throw new ArgumentException($"Not a known Arabic alphabet letter: {originalArabicChar}");
        return result;
    }

    public static string GetSimplifiedArabicString(
        string originalArabicString,
        bool throwErrorOnUnknownCharacter)
    {
        if (string.IsNullOrEmpty(originalArabicString))
            return originalArabicString;
        var result = new StringBuilder();
        foreach (char arabicChar in originalArabicString)
        {
            if (Letters.TryGetValue(arabicChar, out string letter))
                result.Append(letter);
            else if (throwErrorOnUnknownCharacter)
            {
                throw new KeyNotFoundException(arabicChar.ToString());
            }
        }
        return result.ToString();
    }

    static Dictionary<char, string> BuildLetters()
    {
        var letters = new Dictionary<char, string>();
        foreach (char c in AllLetters)
            if (c != ' ')
                letters[c] = c.ToString();
        letters['إ'] = "ا";
        letters['أ'] = "ا";
        letters['آ'] = "ا";
        letters['ة'] = "ت";
        letters['ى'] = "ي";
        letters['ئ'] = "ي";
        letters['ء'] = "";
        letters["اَ".ToCharArray()[1]] = "";
        letters["اِ".ToCharArray()[1]] = "";
        letters["اْ".ToCharArray()[1]] = "";
        letters["اُ".ToCharArray()[1]] = "";
        letters["اّ".ToCharArray()[1]] = "";
        letters["اً".ToCharArray()[1]] = "";
        letters["اٌ".ToCharArray()[1]] = "";
        return letters;
    }
}
