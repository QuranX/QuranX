using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Models
{
	public class Quran_Verses
	{
		public Chapter Chapter { get; private set; }
		public int Verse { get; private set; }
		public int LastVerse { get; private set; }
		public IEnumerable<ChapterAndVerse> Verses { get; private set; }

		public Quran_Verses(int chapter, int verse, int lastVerse)
		{
			QuranVerseHelper.Clip(
				chapter: ref chapter,
				verse: ref verse,
				lastVerse: ref lastVerse
			);
			this.Chapter = SharedData.Document.QuranDocument[chapter];
			this.Verses = Chapter.Verses
				.Where(x => x.Index >= verse && x.Index <= lastVerse)
				.OrderBy(x => x.Index)
				.ToList()
				.ConvertAll(x => new ChapterAndVerse(
					chapter: Chapter, 
					verse: x
				));
			this.Verse = verse;
			this.LastVerse = lastVerse;
		}

	}
}