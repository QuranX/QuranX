using System.Globalization;
using System.IO;
using Lucene.Net.Analysis.Core;
using QuranX.Persistence.Services;

namespace QuranX.Persistence.LuceneSupport
{
	public sealed class QuranXLetterTokenizer : LetterTokenizer
	{
		public QuranXLetterTokenizer(TextReader @in) : base(Consts.LuceneVersion, @in)
		{
		}

		protected override bool IsTokenChar(int c)
		{
			// Convert the code point to a string
			string s = char.ConvertFromUtf32(c);

			// Check if the character is a letter or digit
			bool isLetterOrDigit = char.IsLetterOrDigit(s, 0);

			// Get the Unicode category of the character
			UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(s, 0);

			// Return true if it's a letter, digit, or a non-spacing mark
			return isLetterOrDigit || category == UnicodeCategory.NonSpacingMark;
		}
	}
}
