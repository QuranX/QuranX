namespace QuranX.Persistence.Models
{
	public class HadithIndexDefinition
	{
		public string CollectionCode { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string ValuePrefix { get; }
		public string[] PartNames { get; set; }
		public bool IsPrimary { get; }

		public HadithIndexDefinition(
			string collectionCode, 
			string code, 
			string name, 
			string valuePrefix,
			string[] partNames,
			bool isPrimary)
		{
			CollectionCode = collectionCode;
			Code = code;
			Name = name;
			ValuePrefix = valuePrefix;
			PartNames = partNames;
			IsPrimary = isPrimary;
		}
	}
}
