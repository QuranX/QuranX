using QuranX.DocumentModel;
using System;
using System.Collections.Generic;

namespace QuranX.Models
{
	public class Quran_VerseTafsirs
	{
		public Chapter Chapter { get; private set; }
		public int Verse { get; private set; }
		public IEnumerable<Tuple<Tafsir, TafsirComment>> TafsirComments { get; private set; }

		public Quran_VerseTafsirs(int chapter, int verse)
		{
			QuranVerseHelper.Clip(
				chapter: ref chapter,
				verse: ref verse
			);
			this.Chapter = SharedData.Document.QuranDocument[chapter];
			this.Verse = verse;
			this.TafsirComments = SharedData.Document.TafsirDocument.GetCommentaries(
				chapterIndex: chapter,
				verseIndex: verse
			);
		}
	}
}