namespace QuranX.DocumentModel
{
	public class WordReference
	{
		public readonly string Root;
		public readonly int ChapterIndex;
		public readonly int VerseIndex;
		public readonly int WordIndex;
		public readonly int WordPartIndex;
		public readonly string WordType;
		public readonly string WordTypeDescription;
		public readonly string BuckwalterText;
		public readonly string EnglishText;

		public WordReference(
			string root,
			int chapterIndex,
			int verseIndex,
			int wordIndex,
			int wordPartIndex,
			string wordPartType,
			string wordPartTypeDescription,
			string buckwalterText,
			string englishText)
		{
			this.Root = root;
			this.ChapterIndex = chapterIndex;
			this.VerseIndex = verseIndex;
			this.WordIndex = wordIndex;
			this.WordPartIndex = wordPartIndex;
			this.WordType = wordPartType;
			this.WordTypeDescription = wordPartTypeDescription;
			this.BuckwalterText = buckwalterText;
			this.EnglishText = englishText;
		}

		public override string ToString()
		{
			return string.Format(
					"{0}:{1}.{2}",
					ChapterIndex,
					VerseIndex,
					WordIndex
				);
		}

		public static string GetLocationKey(
			int chapterIndex,
			int verseIndex,
			int wordIndex,
			int wordPartIndex)
		{
			return string.Format("{0}:{1}:{2}:{3}",
					chapterIndex,
					verseIndex,
					wordIndex,
					wordPartIndex
				);
		}

		public string LocationKey
		{
			get
			{
				return GetLocationKey(
						chapterIndex: ChapterIndex,
						verseIndex: VerseIndex,
						wordIndex: WordIndex,
						wordPartIndex: WordPartIndex
					);
			}
		}
	}

}
