using QuranX.DocumentModel;
using System;
using System.Collections.Generic;

namespace QuranX.Models
{
	public class Quran_VerseHadiths
	{
		public Chapter Chapter { get; private set; }
		public int Verse { get; private set; }
		public IEnumerable<CollectionAndHadith> Hadiths { get; private set; }

		public Quran_VerseHadiths(int chapter, int verse)
		{
			QuranVerseHelper.Clip(
				chapter: ref chapter,
				verse: ref verse
			);
			this.Chapter = SharedData.Document.QuranDocument[chapter];
			this.Verse = verse;
			this.Hadiths = SharedData.Document.HadithDocument.GetHadithsForVerse(
				chapterIndex: chapter,
				verseIndex: verse
			);
		}
	}
}