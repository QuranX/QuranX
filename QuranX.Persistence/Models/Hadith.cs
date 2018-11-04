using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Hadith
	{
		public int Id { get; }
		public string CollectionCode { get; }
		public IReadOnlyList<string> ArabicText { get; }
		public IReadOnlyList<string> EnglishText { get; }
		public IReadOnlyList<VerseRangeReference> VerseRangeReferences { get; }
		public IReadOnlyList<HadithReference> References { get; }
		public string PrimaryReferenceCode { get; }
		public string PrimaryReferenceValues;

		public Hadith(
			int id,
			string collectionCode,
			IEnumerable<string> arabicText,
			IEnumerable<string> englishText,
			IEnumerable<VerseRangeReference> verseRangeReferences,
			IEnumerable<HadithReference> references,
			string primaryReferenceCode,
			string primaryReferenceValues)
		{
			Id = id;
			CollectionCode = collectionCode;
			PrimaryReferenceCode = primaryReferenceCode;
			ArabicText = arabicText.ToList().AsReadOnly();
			EnglishText = englishText.ToList().AsReadOnly();
			VerseRangeReferences = verseRangeReferences.ToList().AsReadOnly();
			References = references.ToList().AsReadOnly();
			PrimaryReferenceValues = primaryReferenceValues;
		}
	}
}
