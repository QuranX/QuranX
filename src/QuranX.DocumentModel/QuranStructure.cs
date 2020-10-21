using System.Xml.Linq;
using System.Linq;
using System;

namespace QuranX.DocumentModel
{
	public static class QuranStructure
	{
		static readonly int[] _VerseCount;
		static readonly string[] _ArabicVerseNames;
		static readonly string[] _EnglishVerseNames;

		static QuranStructure()
		{
			_VerseCount = new int[114];
			_ArabicVerseNames = new string[114];
			_EnglishVerseNames = new string[114];
			var doc = XDocument.Parse(QuranX.DocumentModel.Properties.Resources.quran_uthmani);
			foreach (XElement chapterNode in doc.Document.Descendants("sura"))
			{
				int chapterIndex = int.Parse(chapterNode.Attribute("index").Value);
				int verseCount = chapterNode.Descendants("aya").Count();
				_VerseCount[chapterIndex - 1] = verseCount;
				_ArabicVerseNames[chapterIndex - 1] = chapterNode.Attribute("name").Value;
			}

			doc = XDocument.Parse(QuranX.DocumentModel.Properties.Resources.QuranStructure);
			foreach (XElement chapterNode in doc.Document.Descendants("chapter"))
			{
				int chapterIndex = int.Parse(chapterNode.Attribute("index").Value);
				string chapterName = chapterNode.Attribute("name").Value;
				_EnglishVerseNames[chapterIndex - 1] = chapterName;
			}
		}

		public static void ValidateChapterAndVerse(int chapter, int verse)
		{
			if (chapter < 1 || chapter > 114)
				throw new ArgumentOutOfRangeException("Chapter");
			if (verse < 1 || verse > QuranStructure.VerseCount(chapter))
				throw new ArgumentOutOfRangeException("Verse");
		}

		public static int VerseCount(int chapterIndex)
		{
			return _VerseCount[chapterIndex - 1];
		}

		public static string ArabicChapterName(int chapterIndex)
		{
			return _ArabicVerseNames[chapterIndex - 1];
		}

		public static string EnglishChapterName(int chapterIndex)
		{
			return _EnglishVerseNames[chapterIndex - 1];
		}

	}
}
