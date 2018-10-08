using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class HadithReferenceDefinition :
		IComparable,
		IComparable<HadithReferenceDefinition>
	{
		public bool IsPrimary { get; private set; }
		public string Code { get; private set; }
		public string Name { get; private set; }
		public string ValuePrefix { get; private set; }
		public string[] PartNames { get; private set; }

		public HadithReferenceDefinition(bool isPrimary, string code, string name, IEnumerable<string> partNames, string valuePrefix = null)
		{
			if (string.IsNullOrWhiteSpace(code))
				throw new ArgumentNullException(nameof(code));
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			if ((partNames ?? new string[0]).Count() == 0)
				throw new ArgumentException(nameof(partNames), "No part names");
			if (partNames.Any(x => string.IsNullOrWhiteSpace(x)))
				throw new ArgumentException(nameof(partNames), "One or more part names are empty");

			this.IsPrimary = isPrimary;
			this.Code = code;
			this.Name = name;
			this.PartNames = partNames.ToArray();
			this.ValuePrefix = valuePrefix;
		}

		public static bool operator ==(HadithReferenceDefinition first, HadithReferenceDefinition second)
		{
			if (Object.ReferenceEquals(first, null) && Object.ReferenceEquals(second, null))
				return true;
			if (Object.ReferenceEquals(first, null) || Object.ReferenceEquals(second, null))
				return false;
			return first.Equals(second);
		}

		public static bool operator !=(HadithReferenceDefinition first, HadithReferenceDefinition second)
		{
			return !(first == second);
		}

		public override bool Equals(object obj)
		{
			var other = obj as HadithReferenceDefinition;
			if (other == null)
				return false;
			return string.Compare(Code, other.Code, true) == 0;
		}

		public override int GetHashCode()
		{
			return (Code ?? "").GetHashCode();
		}

		int IComparable.CompareTo(object obj)
		{
			return (this as IComparable<HadithReferenceDefinition>).CompareTo((HadithReferenceDefinition)obj);
		}

		int IComparable<HadithReferenceDefinition>.CompareTo(HadithReferenceDefinition other)
		{
			return string.Compare(Code, other.Code, true);
		}
	}
}
