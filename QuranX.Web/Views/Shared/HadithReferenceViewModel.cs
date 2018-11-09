using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Views.Shared
{
	public class HadithReferenceViewModel
	{
		public string CollectionCode { get; }
		public string CollectionName { get; }
		public string IndexCode { get; }
		public string IndexName { get; }
		public IReadOnlyList<KeyValuePair<string, string>> PartNamesAndValues;

		public HadithReferenceViewModel(
			string collectionCode,
			string collectionName,
			string indexCode,
			string indexName,
			IEnumerable<KeyValuePair<string, string>> partNamesAndValues)
		{
			CollectionCode = collectionCode;
			CollectionName = collectionName;
			IndexCode = indexCode;
			IndexName = indexName;
			PartNamesAndValues = partNamesAndValues.ToList().AsReadOnly();
		}


		public string ToUrlPath()
		{
			return string.Join("/", PartNamesAndValues.Select(x => $"{x.Key}-{x.Value}"));
		}

		public override string ToString()
		{
			return ToUrlPath().Replace("/", ", ").Replace("-", " ");
		}
	}
}