using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuranX.Models
{
	public class Analysis_Verse
	{
		public Chapter Chapter { get; private set; }
		public CorpusVerse Verse { get; private set; }

		public Analysis_Verse(int chapter, int verse)
		{
			QuranVerseHelper.Clip(
				chapter: ref chapter,
				verse: ref verse
			);
			this.Chapter = SharedData.Document.QuranDocument[chapter];
			this.Verse = SharedData.Document.CorpusDocument[chapter, verse];
		}
	}
}