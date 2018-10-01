namespace QuranX.Persistence.Models
{
	public class HadithReference
	{
		public string CollectionCode { get; }
		public string IndexCode { get; }
		public int IndexPart1 { get; }
		public string IndexPart1Suffix { get; }
		public int? IndexPart2 { get; }
		public string IndexPart2Suffix { get; }
		public int? IndexPart3 { get; }
		public string IndexPart3Suffix { get; }
		public int HadithId { get; }

		public HadithReference(
			string collectionCode,
			string indexCode,
			int indexPart1,
			string indexPart1Suffix,
			int? indexPart2,
			string indexPart2Suffix,
			int? indexPart3,
			string indexPart3Suffix,
			int hadithId)
		{
			CollectionCode = collectionCode;
			IndexCode = indexCode;
			IndexPart1 = indexPart1;
			IndexPart1Suffix = indexPart1Suffix;
			IndexPart2 = indexPart2;
			IndexPart2Suffix = indexPart2Suffix;
			IndexPart3 = indexPart3;
			IndexPart3Suffix = indexPart3Suffix;
			HadithId = hadithId;
		}
	}
}
