using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class DictionaryEntry
	{
		public string DictionaryCode { get; set; }
		public string Code { get; set; }
		public IReadOnlyList<string> Html { get; set; }

		public DictionaryEntry(string dictionaryCode, string code, IEnumerable<string> html)
		{
			DictionaryCode = dictionaryCode;
			Code = code;
			Html = html.ToList().AsReadOnly();
		}
	}
}
