using System;
using System.Collections.Generic;

namespace QuranX.DocumentModel
{
	public class WordsDocumentVisitor
	{
		protected virtual void VisitDocument(WordsDocument document)
		{
			VisitRootWords(document.WordReferences);
		}

		protected virtual void VisitRootWords(IEnumerable<Word> roots)
		{
			foreach (var root in roots)
				VisitRootWord(root);
		}

		protected virtual void VisitRootWord(Word rootWord)
		{
			VisitRootWordReferences(rootWord.References);
		}

		protected virtual void VisitRootWordReferences(IEnumerable<WordReference> references)
		{
			foreach (var reference in references)
				VisitRootWordReference(reference);
		}

		protected virtual void VisitRootWordReference(WordReference reference)
		{
		}


	}
}
