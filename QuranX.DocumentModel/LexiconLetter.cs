using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
    public class LexiconLetter
    {
        Dictionary<string, LexiconEntry> EntriesByRoot;

        public readonly char Letter;

        public LexiconLetter(char letter)
        {
            this.Letter = letter;
            this.EntriesByRoot = new Dictionary<string, LexiconEntry>();
        }

        public LexiconEntry this[string root]
        {
            get { return EntriesByRoot[root]; }
        }

        public IEnumerable<LexiconEntry> AllEntries()
        {
            return EntriesByRoot
                .OrderBy(x => x.Key)
                .Select(x => x.Value);
        }

        public void AddEntry(LexiconEntry entry)
        {
            EntriesByRoot[entry.Root] = entry;
        }

    }
}
