namespace QuranX.Persistence.Models
{
	public class HadithReference
	{
		public string CollectionCode { get; }
		public string IndexCode { get; }
		public string IndexPart1 { get; }
		public string IndexPart2 { get; }
		public string IndexPart3 { get; }
		public string Suffix { get; }
		public int HadithId { get; }

		public HadithReference(
			string collectionCode,
			string indexCode,
			string indexPart1,
			string indexPart2,
			string indexPart3,
			string suffix,
			int hadithId)
		{
			CollectionCode = collectionCode;
			IndexCode = indexCode;
			IndexPart1 = indexPart1;
			IndexPart2 = indexPart2;
			IndexPart3 = indexPart3;
			Suffix = suffix;
			HadithId = hadithId;
		}
	}
}
