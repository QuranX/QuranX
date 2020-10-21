using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class CorpusVerseWord
	{
		readonly Dictionary<int, CorpusVerseWordPart> _Parts;

		public readonly int Index;
		public readonly string Buckwalter;
		public readonly string English;

		public CorpusVerseWord(
			int index,
			string buckwalter,
			string english)
		{
			this.Index = index;
			this.Buckwalter = buckwalter;
			this.English = english;
			this._Parts = new Dictionary<int, CorpusVerseWordPart>();
		}

		public void AddPart(CorpusVerseWordPart part)
		{
			_Parts.Add(part.Index, part);
		}

		public CorpusVerseWordPart this[int index]
		{
			get
			{
				return _Parts[index];
			}
		}

		public IEnumerable<CorpusVerseWordPart> Parts
		{
			get
			{
				return _Parts
					.Select(x => x.Value)
					.OrderBy(x => x.Index);
			}
		}
	}
}
