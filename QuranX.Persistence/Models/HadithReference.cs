using System;
using System.Text.RegularExpressions;

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

		public static (int index, string suffix) SplitValue(string value)
		{
			var regex = new Regex(@"^(\d+)(\w+)?$");
			Match match = regex.Match(value);
			if (!match.Success)
				throw new ArgumentException("Must be digits alone or digits + letters", nameof(value));
			int index = int.Parse(match.Groups[1].Value);
			string suffix = match.Groups[2].Value;
			return (index, suffix);
		}

		public static (string indexPartName, int index, string suffix) SplitNameAndValue(string value)
		{
			var regex = new Regex(@"^([a-zA-Z]+)-(\d+)(\w+)?$");
			Match match = regex.Match(value);
			if (!match.Success)
				throw new ArgumentException("Must be Name, a dash, and then digits alone or digits + letters", nameof(value));
			string indexPartName = match.Groups[1].Value;
			int index = int.Parse(match.Groups[2].Value);
			string suffix = match.Groups[3].Value;
			return (indexPartName, index, suffix);
		}
	}
}
