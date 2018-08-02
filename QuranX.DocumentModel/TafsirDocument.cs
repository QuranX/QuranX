using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class TafsirDocument
	{
		readonly Dictionary<string, Tafsir> _Tafsirs;

		public TafsirDocument()
		{
			_Tafsirs = new Dictionary<string, Tafsir>(StringComparer.InvariantCultureIgnoreCase);
		}

		public Tafsir this[string code]
		{
			get { return _Tafsirs[code]; }
		}

        public bool TryGetTafsir(string code, out Tafsir tafsir)
        {
            return _Tafsirs.TryGetValue(code, out tafsir);
        }

		public void AddTafsir(Tafsir tafsir)
		{
			_Tafsirs.Add(tafsir.Code, tafsir);
		}

		public IEnumerable<Tafsir> Tafsirs
		{
			get
			{
				return _Tafsirs
					.Select(x => x.Value)
					.OrderBy(x => x.Mufassir);
			}
		}

		public IEnumerable<Tuple<Tafsir, TafsirComment>> GetCommentaries(int chapterIndex, int verseIndex)
		{
			var result =
				from t in Tafsirs
				let c = t.CommentaryForVerse(chapterIndex: chapterIndex, verseIndex: verseIndex)
				where c != null
				select new Tuple<Tafsir, TafsirComment>(t, c);
			return result;
		}

	}
}
