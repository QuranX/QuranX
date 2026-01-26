using System.Collections.Generic;
using System.Linq;
using QuranX.Web.Models;

namespace QuranX.Web.Extensions
{
	public static class ChapterAndVerseSelectionExtensions
	{
		public static string ToDisplayText(this IEnumerable<ChapterAndVerseSelection> selections)
		{
			var items = new List<string>();
			foreach(var selection in selections)
			{
				int firstVerse = selection.Verses.Min(x => x.VerseNumber);
				int lastVerse = selection.Verses.Max(x => x.VerseNumber);
				string lastVerseText = null;
				if (firstVerse != lastVerse)
					lastVerseText = $"-{lastVerse}";
				string text = $"{selection.Chapter.ChapterNumber}.{firstVerse}{lastVerseText} {selection.Chapter.EnglishName} ";
				items.Add(text);
			}
			return string.Join(", ", items);
		}
	}
}
