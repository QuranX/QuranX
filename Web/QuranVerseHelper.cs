using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace QuranX
{
	public static class QuranVerseHelper
	{
		public static void NextVerse(ref int chapter, ref int verse)
		{
			int originalVerse = verse;
			int originalChapter = chapter;
			verse++;
			Clip(chapter: ref chapter, verse: ref verse);
			if (verse == originalVerse)
			{
				verse = 1;
				chapter++;
				Clip(chapter: ref chapter, verse: ref verse);
				if (chapter == originalChapter)
					chapter = 1;
			}
		}

		public static void PreviousVerse(ref int chapter, ref int verse)
		{
			int originalVerse = verse;
			int originalChapter = chapter;
			verse--;
			Clip(chapter: ref chapter, verse: ref verse);
			if (verse == originalVerse)
			{
				verse = 999;
				chapter--;
				Clip(chapter: ref chapter, verse: ref verse);
				if (chapter == originalChapter)
					chapter = 114;
			}
		}

		public static void Clip(ref int chapter, ref int verse)
		{
			int lastVerse = -1;
			Clip(
				chapter: ref chapter,
				verse: ref verse,
				lastVerse: ref lastVerse
			);
		}

		public static void Clip(ref int chapter, ref int verse, ref int lastVerse)
		{
			if (chapter < 1)
				chapter = 1;
			else if (chapter > SharedData.Document.QuranDocument.ChapterCount)
				chapter = SharedData.Document.QuranDocument.ChapterCount;

			if (lastVerse == -1)
				lastVerse = verse;
			ClipVerse(
				chapter: chapter,
				verse: ref verse
			);
			ClipVerse(
				chapter: chapter,
				verse: ref lastVerse
			);
			if (lastVerse < verse)
			{
				int temp = verse;
				verse = lastVerse;
				lastVerse = temp;
			}
		}

		static void ClipVerse(int chapter, ref int verse)
		{
			if (verse == -1 || verse > SharedData.Document.QuranDocument[chapter].VerseCount)
				verse = SharedData.Document.QuranDocument[chapter].VerseCount;
			else if (verse < 1)
				verse = 1;
		}
	}
}