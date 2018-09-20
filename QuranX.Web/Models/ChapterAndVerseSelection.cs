using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class ChapterAndVerseSelection
	{
		public readonly Chapter Chapter;
		public readonly Verse[] Verses;

		public ChapterAndVerseSelection(Chapter chapter, IEnumerable<Verse> verses)
		{
			Chapter = chapter;
			Verses = verses.ToArray();
		}
	}
}