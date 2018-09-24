using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Hadith
	{
		public int Id { get; }
		public IReadOnlyList<string> ArabicText { get; }
		public IReadOnlyList<string> EnglishText { get; }
		public IReadOnlyList<VerseRangeReference> VerseRangeReferences { get; }
		public IReadOnlyList<HadithReference> References { get; }

		public Hadith(
			int id,
			IEnumerable<string> arabicText,
			IEnumerable<string> englishText,
			IEnumerable<VerseRangeReference> verseRangeReferences,
			IEnumerable<HadithReference> references)
		{
			Id = id;
			ArabicText = arabicText.ToList().AsReadOnly();
			EnglishText = englishText.ToList().AsReadOnly();
			VerseRangeReferences = verseRangeReferences.ToList().AsReadOnly();
			References = references.ToList().AsReadOnly();
		}
	}
}
