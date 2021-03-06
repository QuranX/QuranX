﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysisWord
	{
		public int WordNumber { get; }
		public string English { get; }
		public string Buckwalter { get; }
		public IReadOnlyList<VerseAnalysisWordPart> WordParts { get; }

		public VerseAnalysisWord(
			int wordNumber,
			string english,
			string buckwalter,
			IEnumerable<VerseAnalysisWordPart> wordParts)
		{
			if (wordParts == null)
				throw new ArgumentNullException(nameof(wordParts));

			WordNumber = wordNumber;
			English = english;
			Buckwalter = buckwalter;
			WordParts = wordParts.ToList().AsReadOnly();
		}
	}
}
