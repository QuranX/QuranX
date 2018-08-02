using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
    public class Lexicon
    {
        public readonly string Code;
        public readonly string AuthorName;
        readonly Dictionary<char, LexiconLetter> LettersByAlphabet;

        public Lexicon(string code, string name)
        {
            this.Code = code;
            this.AuthorName = name;
            this.LettersByAlphabet = new Dictionary<char, LexiconLetter>();
        }

        public LexiconLetter this[char letter]
        {
            get
            {
                return LettersByAlphabet[letter];
            }
        }

        public LexiconEntry this[string root]
        {
            get
            {
                var letter = this[root[0]];
                return letter[root];
            }
        }

        public void AddEntry(LexiconEntry entry)
        {
            var firstLetter = entry.Root[0];
            LexiconLetter letter;
            if (!LettersByAlphabet.TryGetValue(firstLetter, out letter))
            {
                letter = new LexiconLetter(firstLetter);
                LettersByAlphabet.Add(firstLetter, letter);
            }
            letter.AddEntry(entry);
        }

        public IEnumerable<LexiconLetter> AllLetters()
        {
            return LettersByAlphabet
                .OrderBy(x => x.Key)
                .Select(x => x.Value);
        }

        public IEnumerable<LexiconEntry> AllEntries()
        {
            return LettersByAlphabet.Values
                .SelectMany(x => x.AllEntries())
                .OrderBy(x => x.Root);
        }

    }
}
