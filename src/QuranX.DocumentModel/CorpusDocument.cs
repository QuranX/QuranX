using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel
{
	public class CorpusDocument
	{
		readonly Dictionary<VerseReference, CorpusVerse> _verses;

		public CorpusDocument()
		{
			_verses = new Dictionary<VerseReference, CorpusVerse>();
		}

		public IEnumerable<CorpusVerse> Verses
		{
			get
			{
				return _verses
					.Select(x => x.Value)
					.OrderBy(x => x.Reference);
			}
		}

		public CorpusVerse this[int chapterIndex, int verseIndex]
		{
			get
			{
				var verseReference = new VerseReference(
						chapter: chapterIndex,
						verse: verseIndex
					);
				return _verses[verseReference];
			}
		}

		public void AddVerse(CorpusVerse verse)
		{
			_verses.Add(verse.Reference, verse);
		}


	}

}
