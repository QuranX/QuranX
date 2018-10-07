using System;
using System.Text.RegularExpressions;

namespace QuranX.Persistence.Models
{
	public class HadithReference : IComparable<HadithReference>
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

		public override int GetHashCode()
		{
			string hashString =
				$"{CollectionCode}/{IndexCode}"
				+ $"/{IndexPart1}/{IndexPart1Suffix}"
				+ $"/{IndexPart2}/{IndexPart2Suffix}"
				+ $"/{IndexPart3}/{IndexPart3Suffix}";
			return hashString.GetHashCode();

		}

		public override bool Equals(object obj)
		{
			HadithReference other = obj as HadithReference;
			if (other == null)
				return false;

			return
				string.Compare(CollectionCode, other.CollectionCode, true) == 0
				&& string.Compare(IndexCode, other.IndexCode, true) == 0
				&& IndexPart1 == other.IndexPart1
				&& string.Compare(IndexPart1Suffix, other.IndexPart1Suffix, true) == 0
				&& IndexPart2 == other.IndexPart2
				&& string.Compare(IndexPart2Suffix, other.IndexPart2Suffix, true) == 0
				&& IndexPart1 == other.IndexPart3
				&& string.Compare(IndexPart3Suffix, other.IndexPart3Suffix, true) == 0;
		}

		public int CompareTo(HadithReference other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));
			int result;
			if ((result = string.Compare(CollectionCode, other.CollectionCode, true)) != 0)
				return result;
			if ((result = string.Compare(IndexCode, other.IndexCode, true)) != 0)
				return result;
			if ((result = other.IndexPart1 - IndexPart1) != 0)
				return result;
			if ((result = string.Compare(IndexPart1Suffix, other.IndexPart1Suffix, true)) != 0)
				return result;
			if ((result = other.IndexPart2 ?? 0 - IndexPart2 ?? 0) != 0)
				return result;
			if ((result = string.Compare(IndexPart2Suffix, other.IndexPart2Suffix, true)) != 0)
				return result;
			if ((result = other.IndexPart3 ?? 0 - IndexPart3 ?? 0) != 0)
				return result;
			if ((result = string.Compare(IndexPart3Suffix, other.IndexPart3Suffix, true)) != 0)
				return result;
			return 0;
		}

		public HadithReference ExcludingFinalSuffix()
		{
			int value1 = IndexPart1;
			string value1Suffix = IndexPart2.HasValue ? IndexPart1Suffix : null;
			int? value2 = IndexPart2;
			string value2Suffix = IndexPart2.HasValue ? IndexPart2Suffix : null;
			int? value3 = IndexPart3;
			string value3Suffix = null; // This can never be anything other than null
			return new HadithReference(
				collectionCode: CollectionCode,
				indexCode: IndexCode,
				indexPart1: value1,
				indexPart1Suffix: value1Suffix,
				indexPart2: value2,
				indexPart2Suffix: value2Suffix,
				indexPart3: value3,
				indexPart3Suffix: value3Suffix,
				hadithId: -1);
		}

		public static bool operator ==(HadithReference a, HadithReference b)
		{
			if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
				return true;
			return a.Equals(b);
		}

		public static bool operator !=(HadithReference a, HadithReference b)
		{
			return !(a == b);
		}
	}
}
