using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuranX.Models
{
	public class Tafsir_Commentary
	{
		public Tafsir Tafsir { get; private set; }
		public Chapter Chapter { get; private set; }
		public int Verse { get; private set; }
		public TafsirComment Commentary;

		public Tafsir_Commentary(string tafsirCode, int chapter, int verse)
		{
			QuranVerseHelper.Clip(
				chapter: ref chapter,
				verse: ref verse
			);
			this.Tafsir = SharedData.Document.TafsirDocument[tafsirCode];
			this.Chapter = SharedData.Document.QuranDocument[chapter];
			this.Verse = verse;
			this.Commentary = Tafsir.CommentaryForVerse(chapter, verse);
		}
	}

}