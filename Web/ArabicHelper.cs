using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace QuranX
{
	public static class ArabicHelper
	{
		const string ArabicAlphabet = "إ أ آ ا ب ت ث ج ح خ د ذ ر ز س ش ص ض ط ظ ع غ ف ق ك ل م ن ه و ي ؤ ء ة ئ ى ";
		static HashSet<char> PermittedChars;
		public static readonly Dictionary<char, string> LetterNames;
		public static readonly Dictionary<string, char> LettersByName;
		public static readonly Dictionary<char, char> BlockedChars;
		public static readonly Dictionary<char, char> AlternateChars;

		static ArabicHelper()
		{
			LetterNames = new Dictionary<char, string>();
			PopulateLetterNames();
			LettersByName = LetterNames.ToDictionary(x => x.Value, x => x.Key);
			PermittedChars = new HashSet<char>(ArabicAlphabet.ToCharArray().Distinct());
			BlockedChars = new Dictionary<char, char>();
			AlternateChars = new Dictionary<char, char>();
			PopulateAlternateChars();
		}

		public static string ArabicToLetterNames(string source)
		{
			source = Standardize(source);
			bool isFirst = true;
			var builder = new StringBuilder();
			foreach (char arabicChar in source)
			{
				string letterName;
				if (!LetterNames.TryGetValue(arabicChar, out letterName))
				{
					throw new ArgumentException("No letter name for: " + arabicChar);
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

		public static string LetterNamesToArabic(string source)
		{
			var builder = new StringBuilder();
			string[] parts = source.Split('-');
			foreach (string letterName in parts)
				builder.Append(LettersByName[letterName]);
			return builder.ToString();
		}

		public static string Standardize(string arabicText)
		{
			if (string.IsNullOrEmpty(arabicText))
				return arabicText;

			var resultBuilder = new StringBuilder();
			foreach (char c in arabicText.ToCharArray())
			{
				char alternateChar = GetArabicSubstituteChar(c);
				if (PermittedChars.Contains(alternateChar))
					resultBuilder.Append(alternateChar);
				else
					BlockedChars[c] = c;
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

		static void PopulateAlternateChars()
		{
			AlternateChars['ٱ'] = 'ا';
		}

		static char GetArabicSubstituteChar(char c)
		{
			char alternateChar;
			if (AlternateChars.TryGetValue(c, out alternateChar))
				return alternateChar;
			return c;
		}

		static void PopulateLetterNames()
		{
			LetterNames['ا'] = "alif";
			LetterNames['ب'] = "ba";
			LetterNames['ت'] = "ta";
			LetterNames['ث'] = "tha";
			LetterNames['ج'] = "jim";
			LetterNames['ح'] = "ha";
			LetterNames['خ'] = "kha";
			LetterNames['د'] = "dal";
			LetterNames['ذ'] = "thal";
			LetterNames['ر'] = "ra";
			LetterNames['ز'] = "zay";
			LetterNames['س'] = "sin";
			LetterNames['ش'] = "shin";
			LetterNames['ص'] = "sad";
			LetterNames['ض'] = "dad";
			LetterNames['ط'] = "tta";
			LetterNames['ظ'] = "dha";
			LetterNames['ع'] = "ayn";
			LetterNames['غ'] = "ghayn";
			LetterNames['ف'] = "fa";
			LetterNames['ق'] = "gaf";
			LetterNames['ك'] = "kaf";
			LetterNames['ل'] = "lam";
			LetterNames['م'] = "mim";
			LetterNames['ن'] = "nun";
			LetterNames['ه'] = "heh";
			LetterNames['و'] = "waw";
			LetterNames['ي'] = "ya";
			LetterNames['ء'] = "";
		}




	}
}