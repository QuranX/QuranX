using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class HadithCollection
	{
		public string Code { get; }
		public string Name { get; }
		public IReadOnlyList<HadithReferenceDefinition> ReferenceDefinitions { get; }
		public int HadithCount { get; }

		public HadithCollection(
			string code,
			string name,
			IEnumerable<HadithReferenceDefinition> referenceDefinitions,
			int hadithCount)
		{
			Code = code;
			Name = name;
			ReferenceDefinitions = referenceDefinitions.ToList().AsReadOnly();
			HadithCount = hadithCount;
		}

		public string PrimaryReferenceCode => ReferenceDefinitions.Single(x => x.IsPrimary).Code;
		public HadithReferenceDefinition GetReferenceDefinition(string referenceCode) => ReferenceDefinitions
			.SingleOrDefault(x => string.Compare(x.Code, referenceCode, true) == 0);
		public HadithReferenceDefinition GetPrimaryReferenceDefinition() =>
			GetReferenceDefinition(PrimaryReferenceCode);

		public IEnumerable<HadithReferenceDefinition> GetPossibleReferenceDefinitionsByPartNames(IEnumerable<string> referencePartNames)
		{
			string requiredValues = string.Join(":", referencePartNames).ToLowerInvariant();
			var result = new List<HadithReferenceDefinition>();

			foreach(var reference in ReferenceDefinitions.OrderBy(x => x.IsPrimary))
			{
				string key = string.Join(":", reference.PartNames);
				if (string.Compare(key, requiredValues, true) == 0)
					result.Add(reference);
			}
			return result;
		}
	}
}
