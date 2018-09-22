using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class ChapterAndVerseRangeReferenceSelection
	{
		public readonly Chapter Chapter;
		public readonly VerseRangeReference[] VerseRangeReferences;

		public ChapterAndVerseRangeReferenceSelection(Chapter chapter, IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			Chapter = chapter;
			VerseRangeReferences = verseRangeReferences.ToArray();
		}
	}
}