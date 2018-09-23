using System.Linq;

namespace QuranX.Persistence.Models
{
	public class HadithCollection
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public HadithIndexDefinition[] IndexDefinitions { get; set; }
		public int HadithCount { get; }
		public string PrimaryIndexCode => IndexDefinitions.Single(x => x.IsPrimary).Code;

		public HadithCollection(
			string code,
			string name,
			HadithIndexDefinition[] indexDefinitions,
			int hadithCount)
		{
			Code = code;
			Name = name;
			IndexDefinitions = indexDefinitions;
			HadithCount = hadithCount;
		}
	}
}
