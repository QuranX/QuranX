using System.Linq;

namespace QuranX.Persistence.Models
{
	public class HadithCollection
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public HadithIndexDefinition[] IndexDefinitions { get; set; }
		public string PrimaryIndexCode { get; private set; }

		public HadithCollection(string code, string name, HadithIndexDefinition[] indexDefinitions)
		{
			Code = code;
			Name = name;
			IndexDefinitions = indexDefinitions;
			PrimaryIndexCode = indexDefinitions.Single(x => x.IsPrimary).Code;
		}
	}
}
