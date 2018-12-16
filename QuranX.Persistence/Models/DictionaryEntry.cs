using System.Collections.Generic;
using System.Linq;
using QuranX.Shared;

namespace QuranX.Persistence.Models
{
	public class DictionaryEntry
	{
		public string DictionaryCode { get; }
		public string Word { get; }
		public IReadOnlyList<string> Html { get; }

		public DictionaryEntry(string dictionaryCode, string word, IEnumerable<string> html)
		{
			DictionaryCode = dictionaryCode;
			Word = ArabicHelper.SubstituteAndOmit(word);
			Html = html.ToList().AsReadOnly();
		}
	}
}
