using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysis
	{
		public int ChapterNumber { get; }
		public int VerseNumber { get; }
		public IReadOnlyCollection<VerseAnalysisWord> Words { get; }
		public IReadOnlyCollection<string> Roots => GetRoots();

		private IReadOnlyCollection<string> _roots;

		public VerseAnalysis(
			int chapterNumber,
			int verseNumber,
			IReadOnlyCollection<VerseAnalysisWord> words)
		{
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			Words = words.ToList().AsReadOnly();
		}

		private IReadOnlyCollection<string> GetRoots()
		{
			if (_roots == null)
			{
				_roots = Words
					.SelectMany(x => x.WordParts)
					.Where(x => !string.IsNullOrWhiteSpace(x.Root))
					.Select(x => x.Root)
					.Distinct()
					.ToList()
					.AsReadOnly();
			}
			return _roots;
		}
	}
}
