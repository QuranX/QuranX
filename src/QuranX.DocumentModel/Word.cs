using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class Word
	{
		readonly List<WordReference> _References;
		public readonly string Text;

		public Word(string text, IEnumerable<WordReference> references)
		{
			this.Text = text;
			this._References = references
				.OrderBy(x => x.ChapterIndex)
				.ThenBy(x => x.VerseIndex)
				.ThenBy(x => x.WordIndex)
				.ThenBy(x => x.WordPartIndex)
				.ToList();
		}

		public IEnumerable<WordReference> References
		{
			get
			{
				return _References.AsReadOnly();
			}
		}

	}
}
