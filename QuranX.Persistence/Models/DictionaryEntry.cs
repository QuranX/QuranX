using System.Collections.Generic;
using System.Linq;
using QuranX.Shared;

namespace QuranX.Persistence.Models
{
	public class DictionaryEntry
	{
		public string DictionaryCode { get; }
		public string Word { get; }
		public int EntryIndex { get; }
		public IReadOnlyList<string> Html { get; }

		public DictionaryEntry(string dictionaryCode, string word, int entryIndex, IEnumerable<string> html)
		{
			DictionaryCode = dictionaryCode;
			EntryIndex = entryIndex;
			Word = ArabicHelper.SubstituteAndOmit(word);
			Html = html.ToList().AsReadOnly();
		}
	}
}
