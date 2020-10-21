using DictionaryVM = QuranX.Persistence.Models.Dictionary;
using DictionaryEntryVM = QuranX.Persistence.Models.DictionaryEntry;
using System.Collections.Generic;

namespace QuranX.Web.Views.DictionaryEntry
{
	public class ViewModel
	{
		public string ArabicWord { get; }
		public DictionaryVM Dictionary { get; }
		public IEnumerable<DictionaryEntryVM> Entries { get; }

		public ViewModel(string arabicWord, DictionaryVM dictionary, IEnumerable<DictionaryEntryVM> entries)
		{
			ArabicWord = arabicWord;
			Dictionary = dictionary;
			Entries = entries;
		}
	}
}