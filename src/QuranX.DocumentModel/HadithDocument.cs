using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class HadithDocument
	{
		readonly Dictionary<string, HadithCollection> _Collections;

		public HadithDocument()
		{
			this._Collections = new Dictionary<string, HadithCollection>(StringComparer.InvariantCultureIgnoreCase);
		}

		public HadithCollection this[string code]
		{
			get
			{
				return _Collections[code];
			}
		}

		public IEnumerable<HadithCollection> Collections
		{
			get
			{
				return _Collections
					.Select(x => x.Value)
					.OrderBy(x => x.Name);
			}
		}

		public void AddCollection(HadithCollection collection)
		{
			this._Collections.Add(collection.Code, collection);
		}

		public IEnumerable<CollectionAndHadith> GetHadithsForVerse(
			int chapterIndex,
			int verseIndex)
		{
			var result =
				from c in Collections
				from h in c.GetHadithsForVerse(chapterIndex: chapterIndex, verseIndex: verseIndex)
				select new CollectionAndHadith(collection: c, hadith: h);
			return result;
		}

	}
}
