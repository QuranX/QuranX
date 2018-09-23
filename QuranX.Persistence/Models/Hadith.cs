using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Hadith
	{
		public int Id { get; set; }
		public string[] ArabicText { get; set; }
		public string[] EnglishText { get; set; }
		public VerseRangeReference[] VerseRangeReferences { get; set; }
	}
}
