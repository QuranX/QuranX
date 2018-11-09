using System.Collections.Generic;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class ChapterAndVerseSelection
	{
		public readonly Chapter Chapter;
		public readonly IEnumerable<Verse> Verses;

		public ChapterAndVerseSelection(Chapter chapter, IEnumerable<Verse> verses)
		{
			Chapter = chapter;
			Verses = verses;
		}
	}
}