namespace QuranX.Persistence.Models
{
	public class HadithIndexEntry
	{
		public string CollectionCode { get; set; }
		public string IndexCode { get; set; }
		public int IndexPart1 { get; set; }
		public int IndexPart2 { get; set; }
		public int IndexPart3 { get; set; }
		public string Suffix { get; set; }
		public int HadithId { get; set; }

		public HadithIndexEntry(
			string collectionCode,
			string indexCode,
			int indexPart1,
			int indexPart2,
			int indexPart3,
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
