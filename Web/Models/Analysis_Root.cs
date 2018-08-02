using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuranX.Models
{
	public class Analysis_Root
	{
		readonly Word Word;
		public string ArabicRoot { get; private set; }
		public string LetterNames { get; private set; }
		public Analysis_WordUsageWithVerseExtract[] WordUseages { get; private set; }

		public Analysis_Root(string arabicRoot)
		{
			if (string.IsNullOrEmpty(arabicRoot))
				throw new ArgumentNullException();

			this.ArabicRoot = arabicRoot;
			this.Word = SharedData.Document.RootWordsDocument[ArabicRoot];
			this.LetterNames = QuranX.ArabicHelper.ArabicToLetterNames(ArabicRoot);
			this.WordUseages =
				Word.References
				.OrderBy(x => x.ChapterIndex)
                .ThenBy(x => x.VerseIndex)
				.Select(x => new Analysis_WordUsageWithVerseExtract(x))
				.ToArray();
		}
	}

	public class Analysis_WordUsageWithVerseExtract
	{
		public Analysis_VerseExtract Extract { get; private set; }
        public WordReference Reference { get; private set; }

		public Analysis_WordUsageWithVerseExtract(WordReference wordReference)
		{
			this.Extract = new Analysis_VerseExtract(wordReference);
            this.Reference = wordReference;
		}
	}

	public class Analysis_VerseExtract
	{
		const int NumberOfSurroundingWordsToDisplay = 4;
		public CorpusVerseWord[] PrecedingWords { get; private set; }
		public WordReference WordReference { get; private set; }
		public CorpusVerseWord[] FollowingWords { get; private set; }
        public string BuckwalterWord { get; private set; }

		public Analysis_VerseExtract(WordReference wordReference)
		{
			this.WordReference = wordReference;

			var verseReference = 
				SharedData.Document.CorpusDocument[WordReference.ChapterIndex, WordReference.VerseIndex];
			int wordIndex = wordReference.WordIndex;
			this.PrecedingWords =
				verseReference
				.Words
				.Where(x =>
					x.Index >= wordIndex - NumberOfSurroundingWordsToDisplay
					&& x.Index < wordIndex)
				.ToArray();
			this.FollowingWords =
				verseReference
				.Words
				.Where(x =>
					x.Index > wordIndex
					&& x.Index <= wordIndex + NumberOfSurroundingWordsToDisplay)
				.ToArray();

            BuckwalterWord = wordReference.BuckwalterText;

		}
	}
}