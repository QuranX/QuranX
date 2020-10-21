namespace QuranX.DocumentModel
{
	public class Document
	{
		public readonly QuranDocument QuranDocument;
		public readonly HadithDocument HadithDocument;
		public readonly TafsirDocument TafsirDocument;
		public readonly WordsDocument RootWordsDocument;
		public readonly CorpusDocument CorpusDocument;
		public readonly LexiconDocument LexiconDocument;

		public Document(
			QuranDocument quranDocument,
			HadithDocument hadithDocument,
			TafsirDocument tafsirDocument,
			WordsDocument rootWordsDocument,
			CorpusDocument corpusDocument,
			LexiconDocument lexiconDocument)
		{
			this.QuranDocument = quranDocument;
			this.HadithDocument = hadithDocument;
			this.TafsirDocument = tafsirDocument;
			this.RootWordsDocument = rootWordsDocument;
			this.CorpusDocument = corpusDocument;
			this.LexiconDocument = lexiconDocument;
		}

	}
}
