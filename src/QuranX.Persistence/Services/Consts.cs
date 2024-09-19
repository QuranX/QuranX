using Lucene.Net.Util;

namespace QuranX.Persistence.Services
{
	public static class Consts
	{
		public const string FullTextFieldName = "_FullText";
		public static readonly LuceneVersion LuceneVersion = LuceneVersion.LUCENE_48;
		public const string SerializedObjectFieldName = "_Object";
		public const string SerializedObjectTypeFieldName = "_Type";
	}
}
