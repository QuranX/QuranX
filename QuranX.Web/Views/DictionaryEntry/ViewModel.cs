using DictionaryVM = QuranX.Persistence.Models.Dictionary;
using DictionaryEntryVM = QuranX.Persistence.Models.DictionaryEntry;

namespace QuranX.Web.Views.DictionaryEntry
{
	public class ViewModel
	{
		public DictionaryVM Dictionary { get; }
		public DictionaryEntryVM Entry { get; }

		public ViewModel(DictionaryVM dictionary, DictionaryEntryVM entry)
		{
			Dictionary = dictionary;
			Entry = entry;
		}
	}
}