namespace QuranX.DocumentModel
{
	public class CorpusVerseWordPart
	{
		public readonly int Index;
		public readonly string TypeCode;
		public readonly string Form;
		public readonly string TypeDescription;
		public readonly string Root;
		public readonly string[] Decorators;

		public CorpusVerseWordPart(
			int index,
			string typeCode,
			string form,
			string root,
			string[] decorators)
		{
			this.Index = index;
			this.TypeCode = typeCode;
			this.Form = form;
			this.TypeDescription = WordTypes.Values[typeCode];
			this.Root = root;
			this.Decorators = decorators;
		}

	}
}
