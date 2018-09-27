﻿using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class HadithCollection
	{
		public string Code { get; }
		public string Name { get; }
		public IReadOnlyList<HadithReferenceDefinition> ReferenceDefinitions { get; }
		public int HadithCount { get; }
		public string PrimaryReferenceCode => ReferenceDefinitions.Single(x => x.IsPrimary).Code;

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
	}
}