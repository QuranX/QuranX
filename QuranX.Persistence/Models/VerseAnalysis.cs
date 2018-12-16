using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Shared;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysis
	{
		public int ChapterNumber { get; }
		public int VerseNumber { get; }
		public IReadOnlyList<VerseAnalysisWord> Words { get; }
		public IReadOnlyList<string> RootIndexes => GetRootIndexes();

		private IReadOnlyList<string> _rootIndexes;

		public VerseAnalysis(
			int chapterNumber,
			int verseNumber,
			IReadOnlyList<VerseAnalysisWord> words)
		{
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			Words = words.ToList().AsReadOnly();
		}

		public static string GetIndexForArabicRoot(string root)
			=> ArabicHelper.ArabicToLetterNames(root).Replace('-', 'x');

		private IReadOnlyList<string> GetRootIndexes()
		{
			if (_rootIndexes == null)
			{
				_rootIndexes = Words
					.SelectMany(x => x.WordParts)
					.Where(x => !string.IsNullOrWhiteSpace(x.Root))
					.Select(x => x.Root)
					.Distinct()
					.Select(GetIndexForArabicRoot)
					.ToList()
					.AsReadOnly();
			}
			return _rootIndexes;
		}
	}
}
