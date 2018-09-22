using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.Web.Models
{
	public class ChapterAndVerseReferenceSelection
	{
		public readonly Chapter Chapter;
		public readonly VerseReference[] VerseReferences;

		public ChapterAndVerseReferenceSelection(Chapter chapter, IEnumerable<VerseReference> verseReferences)
		{
			Chapter = chapter;
			VerseReferences = verseReferences.ToArray();
		}
	}
}