using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel
{
	public class CorpusVerse
	{
		readonly Dictionary<int, CorpusVerseWord> WordsByIndex;

		public readonly VerseReference Reference;
		public readonly string ArabicText;

		public CorpusVerse(
			VerseReference reference, 
			string arabicText)
		{
			this.Reference = reference;
			this.ArabicText = arabicText;
			this.WordsByIndex = new Dictionary<int, CorpusVerseWord>();
		}

		public int Index
		{
			get { return Reference.Verse; }
		}

		public void AddWord(CorpusVerseWord word)
		{
			WordsByIndex.Add(word.Index, word);
		}

		public CorpusVerseWord this[int index]
		{
			get { return WordsByIndex[index]; }
		}

		public IEnumerable<CorpusVerseWord> Words
		{
			get
			{
				return WordsByIndex
					.Select(x => x.Value)
					.OrderBy(x => x.Index);
			}
		}
	}
}
