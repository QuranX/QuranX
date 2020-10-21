using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
    public class LexiconDocumentVisitor
    {
        protected virtual void VisitDocument(LexiconDocument document)
        {
            VisitLexicons(document.AllLexicons());
        }

        protected virtual void VisitLexicons(IEnumerable<Lexicon> lexicons)
        {
            foreach (var lexicon in lexicons)
                VisitLexicon(lexicon);
        }

        protected virtual void VisitLexicon(Lexicon lexicon)
        {
            VisitLetters(lexicon.AllLetters());
        }

        protected virtual void VisitLetters(IEnumerable<LexiconLetter> letters)
        {
            foreach (var letter in letters)
                VisitLetter(letter);
        }

        protected virtual void VisitLetter(LexiconLetter letter)
        {
            VisitEntries(letter.AllEntries());
        }

        protected void VisitEntries(IEnumerable<LexiconEntry> entries)
        {
            foreach (var entry in entries)
                VisitEntry(entry);
        }

        protected virtual void VisitEntry(LexiconEntry entry)
        {
        }
    }
}
