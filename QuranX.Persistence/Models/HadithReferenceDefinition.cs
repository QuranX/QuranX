using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class HadithReferenceDefinition
	{
		public string CollectionCode { get; }
		public string Code { get; }
		public string Name { get; }
		public string ValuePrefix { get; }
		public IReadOnlyList<string> PartNames { get; }
		public bool IsPrimary { get; }

		public HadithReferenceDefinition(
			string collectionCode, 
			string code, 
			string name, 
			string valuePrefix,
			IEnumerable<string> partNames,
			bool isPrimary)
		{
			CollectionCode = collectionCode;
			Code = code;
			Name = name;
			ValuePrefix = valuePrefix;
			PartNames = partNames.ToList().AsReadOnly();
			IsPrimary = isPrimary;
		}

		public bool PatternMatch(IEnumerable<string> names)
		{
			IEnumerable<string> subsetOfDefinitionPartNames = PartNames.Take(names.Count());
			return Enumerable.SequenceEqual(subsetOfDefinitionPartNames, names, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
