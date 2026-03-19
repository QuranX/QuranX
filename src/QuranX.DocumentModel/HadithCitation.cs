using System;
using System.Collections.Generic;

namespace QuranX.DocumentModel;

public class HadithCitation : HadithReference,
    IComparable,
    IComparable<HadithCitation>
{
    public string CollectionCode { get; private set; }

    public HadithCitation(
        string collectionCode,
        string code,
        IEnumerable<string> values,
        string suffix
    ) : base(code, values, suffix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(collectionCode);
        CollectionCode = collectionCode;
    }

    public HadithCitation(
        string collectionCode,
        HadithReference reference
    )
        : base(reference?.Code, reference?.Values, reference?.Suffix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(collectionCode);
        CollectionCode = collectionCode;
    }

    public int CompareTo(HadithCitation other)
    {
        return (this as IComparable).CompareTo(other);
    }

    public override string ToString()
    {
        return CollectionCode + " " + base.ToString();
    }

    int IComparable.CompareTo(object obj)
    {
        var other = (HadithCitation)obj;

        int collectionCompare = string.Compare(CollectionCode, other.CollectionCode, true);
        if (collectionCompare != 0)
            return collectionCompare;

        return base.CompareTo(other);
    }

    public static bool operator ==(HadithCitation first, HadithCitation second)
    {
        if (Object.ReferenceEquals(first, null) && Object.ReferenceEquals(second, null))
            return true;
        if (Object.ReferenceEquals(first, null) || Object.ReferenceEquals(second, null))
            return false;
        return first.CompareTo(second) == 0;
    }

    public static bool operator !=(HadithCitation first, HadithCitation second)
    {
        return !(first == second);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is HadithCitation))
            return false;
        return ((HadithCitation)obj).CompareTo(this) == 0;
    }

    public override int GetHashCode() => HashCode.Combine(CollectionCode, base.GetHashCode());
}