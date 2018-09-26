using System.Collections.Generic;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class ChapterAndVerseRangeReferenceSelection
	{
		public readonly Chapter Chapter;
		public readonly IEnumerable<VerseRangeReference> VerseRangeReferences;

		public ChapterAndVerseRangeReferenceSelection(Chapter chapter, IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			Chapter = chapter;
			VerseRangeReferences = verseRangeReferences;
		}
	}
}