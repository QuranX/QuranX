using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
	public class CorpusDocument
	{
		readonly Dictionary<VerseReference, CorpusVerse> _Verses;

		public CorpusDocument()
		{
			this._Verses = new Dictionary<VerseReference, CorpusVerse>();
		}

		public IEnumerable<CorpusVerse> Verses
		{
			get
			{
				return _Verses
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
				return _Verses[verseReference];
			}
		}

		public void AddVerse(CorpusVerse verse)
		{
			_Verses.Add(verse.Reference, verse);
		}


	}

}
