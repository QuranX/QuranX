namespace QuranX.Shared.Models
{
	public class ChapterInfo
	{
		public readonly int ChapterNumber;
		public readonly string ArabicName;
		public readonly string EnglishName;
		public readonly string Period;
		public readonly int NumberOfVerses;
		public readonly int RevelationOrder;

		public ChapterInfo(int chapterNumber, string arabicName, string englishName, string period, int numberOfVerses, int revelationOrder)
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
