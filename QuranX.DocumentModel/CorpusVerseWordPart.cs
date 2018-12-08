namespace QuranX.DocumentModel
{
	public class CorpusVerseWordPart
	{
		public readonly int Index;
		public readonly string TypeCode;
		public readonly int? SubType;
		public readonly string TypeDescription;
		public readonly string Root;
		public readonly string[] Decorators;

		public CorpusVerseWordPart(
			int index,
			string typeCode,
			int? subType,
			string root,
			string[] decorators)
		{
			this.Index = index;
			this.TypeCode = typeCode;
			this.SubType = subType;
			this.TypeDescription = WordTypes.Values[typeCode];
			this.Root = root;
			this.Decorators = decorators;
		}

	}
}
