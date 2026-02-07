using System.Collections.Generic;
using DictionaryEntryVM = QuranX.Persistence.Models.DictionaryEntry;
using DictionaryVM = QuranX.Persistence.Models.Dictionary;

namespace QuranX.Web.Views.DictionaryEntry;

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