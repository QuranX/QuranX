using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranX.DocumentModel
{
	public static class ArabicAlphabet
	{
		const string AllLetters = "إ أ آ ا ب ت ث ج ح خ د ذ ر ز س ش ص ض ط ظ ع غ ف ق ك ل م ن ه و ي ؤ ء ة ئ ى ";
		static Dictionary<char, string> Letters;

		static ArabicAlphabet()
		{
			Letters = new Dictionary<char, string>();
			foreach (char c in AllLetters)
				if (c != ' ')
					Letters[c] = c.ToString();
			//Substitute letters
			Letters['إ'] = "ا";
			Letters['أ'] = "ا";
			Letters['أ'] = "ا";
			Letters['آ'] = "ا";
			Letters['ة'] = "ت";
			Letters['ى'] = "ي";
			Letters['ئ'] = "ي";
			Letters['ء'] = "";
			Letters["اَ".ToCharArray()[1]] = "";
			Letters["اِ".ToCharArray()[1]] = "";
			Letters["اَ".ToCharArray()[1]] = "";
			Letters["اْ".ToCharArray()[1]] = "";
			Letters["اُ".ToCharArray()[1]] = "";
			Letters["اّ".ToCharArray()[1]] = "";
			Letters["اً".ToCharArray()[1]] = "";
			Letters["اٌ".ToCharArray()[1]] = "";
		}

		public static string GetSimplifiedArabicChar(char originalArabicChar)
		{
			string result;
			if (!Letters.TryGetValue(originalArabicChar, out result))
				throw new ArgumentException("Not a known Arabic alphabet letter: " + originalArabicChar);
			return result;	
		}				  

		public static string GetSimplifiedArabicString(string originalArabicString, bool throwErrorOnUnknownCharacter)
		{
			if (string.IsNullOrEmpty(originalArabicString))
				return originalArabicString;
			var result = new StringBuilder();
            foreach (char arabicChar in originalArabicString)
            {
                string letter;
                if (Letters.TryGetValue(arabicChar, out letter))
                    result.Append(Letters[arabicChar]);
                else if (throwErrorOnUnknownCharacter)
                {
                    throw new KeyNotFoundException(arabicChar.ToString());
                }
            }
			return result.ToString();
		}
																  
	}
}
