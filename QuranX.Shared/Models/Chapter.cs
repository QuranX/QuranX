namespace QuranX.Shared.Models
{
	public class Chapter
	{
		public int ChapterNumber { get; set; }
		public string ArabicName { get; set; }
		public string EnglishName { get; set; }
		public string Period { get; set; }
		public int NumberOfVerses { get; set; }
		public int RevelationOrder { get; set; }

		public Chapter() { }

		public Chapter(int chapterNumber, string arabicName, string englishName, string period, int numberOfVerses, int revelationOrder)
		{
			ChapterNumber = chapterNumber;
			ArabicName = arabicName;
			EnglishName = englishName;
			Period = period;
			NumberOfVerses = numberOfVerses;
			RevelationOrder = revelationOrder;
		}
	}
}
