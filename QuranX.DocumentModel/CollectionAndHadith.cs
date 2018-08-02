using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
	public class CollectionAndHadith : IComparable
	{
		public HadithCollection Collection { get; private set; }
		public Hadith Hadith { get; private set; }

		public CollectionAndHadith(HadithCollection collection, Hadith hadith)
		{
			this.Collection = collection;
			this.Hadith = hadith;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CollectionAndHadith))
				return false;

			var other = (CollectionAndHadith)obj;
            return
                this.Collection.Code == other.Collection.Code
                && this.Hadith.PrimaryReference == other.Hadith.PrimaryReference;
		}

		public static bool operator ==(CollectionAndHadith left, CollectionAndHadith right)
		{
			return (left.Equals(right));
		}

		public static bool operator !=(CollectionAndHadith left, CollectionAndHadith right)
		{
			return (!left.Equals(right));
		}

		public int CompareTo(CollectionAndHadith other)
		{
			int stringCompare = string.Compare(this.Collection.Code, other.Collection.Code);
			if (stringCompare != 0)
				return stringCompare;
			return this.Hadith.PrimaryReference.CompareTo(other.Hadith.PrimaryReference);
		}

		public override int GetHashCode()
		{
			unchecked
			{
                return Collection.Code.GetHashCode() + Hadith.PrimaryReference.GetHashCode();
			}
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is CollectionAndHadith))
				throw new ArgumentException();
			return CompareTo((CollectionAndHadith)obj);
		}
	}
}
