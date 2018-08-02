using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
    public class LexiconDocument
    {
        readonly Dictionary<string, Lexicon> LexiconsByAuthorCode;

        public LexiconDocument()
        {
            this.LexiconsByAuthorCode = new Dictionary<string, Lexicon>(StringComparer.InvariantCultureIgnoreCase);
        }

        public Lexicon this[string authorCode]
        {
            get
            {
                return LexiconsByAuthorCode[authorCode];
            }
        }

        public void AddLexicon(Lexicon lexicon)
        {
            LexiconsByAuthorCode.Add(lexicon.Code, lexicon);
        }

        public IEnumerable<Lexicon> AllLexicons()
        {
            return LexiconsByAuthorCode
                .Values
                .OrderBy(x => x.AuthorName);
        }
    }
}
