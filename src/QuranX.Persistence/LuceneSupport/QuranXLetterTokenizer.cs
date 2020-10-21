using System.Globalization;
using System.IO;
using Lucene.Net.Analysis;

namespace QuranX.Persistence.LuceneSupport
{
	public class QuranXLetterTokenizer : LetterTokenizer
	{
		public QuranXLetterTokenizer(TextReader @in) : base(@in)
		{
		}

		protected override bool IsTokenChar(char c)
			=> char.IsLetterOrDigit(c) || char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark;
	}
}
