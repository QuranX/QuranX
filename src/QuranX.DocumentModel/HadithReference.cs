using QuranX.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel;

public class HadithReference :
    IComparable,
    IComparable<HadithReference>,
    IEnumerable<string>,
    IGetDisplayText
{
    public string Code { get; private set; }
    public string[] Values { get; private set; }
    public string Suffix { get; private set; }

    public HadithReference(string code, IEnumerable<string> values, string suffix)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code));
        if (values is null || values.Count() == 0 || values.Any(x => string.IsNullOrWhiteSpace(x)))
            throw new ArgumentException(nameof(values), "Must be an array of non-empty values");

        Code = code;
        Values = values.ToArray();
        Suffix = suffix?.ToLowerInvariant();
    }

    public int Length
    {
        get { return Values.Length; }
    }

    public string this[int index]
    {
        get { return Values[index]; }
    }

    public string GetCaption(IEnumerable<string> referencePartNames)
    {
        var captionParts = referencePartNames.ToArray();
        int index = -1;
        foreach (string referencePartName in referencePartNames)
        {
            index++;
            captionParts[index] = string.Format(
                    "{0} {1}",
                    referencePartName,
                    Values[index]
                );
        }
        return string.Join(", ", captionParts) + Suffix;
    }

    public int CompareTo(HadithReference other)
    {
        return (this as IComparable).CompareTo(other);
    }

    public bool IsPartialMatch(string[] patternValues)
    {
        if (patternValues is null || patternValues.Length != Values.Length)
            return false;
        for (int i = 0; i < patternValues.Length; i++)
        {
            string referencePartValue = Values[i];
            string patternPartValue = patternValues[i];
            if (patternPartValue != "*" && patternPartValue != referencePartValue)
                return false;
        }
        return true;
    }

    public override string ToString() => GetDisplayText();

    public string GetDisplayText()
    {
        return Code + " " +
            string.Join(
                separator: ".",
                values: (IEnumerable<string>)Values
            ) + Suffix;
    }

    int IComparable.CompareTo(object obj)
    {
        var other = (HadithReference)obj;

        int codeCompare = string.Compare(Code, other.Code, true);
        if (codeCompare != 0)
            return codeCompare;

        int length = Math.Min(Length, other.Length);

        for (int index = 0; index < length; index++)
        {
            string left = this[index];
            string right = other[index];
            if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
            {
                if (leftInt != rightInt)
                    return leftInt.CompareTo(rightInt);
            }
            else
            {
                int padLength = Math.Max(left.Length, right.Length);
                left = left.PadRight(padLength, '0');
                right = right.PadRight(padLength, '0');
                if (left != right)
                    return string.CompareOrdinal(left, right);
            }
        }
        if (this.Length < other.Length)
            return -1;
        if (this.Length > other.Length)
            return 1;
        return string.Compare(this.Suffix ?? "", other.Suffix ?? "", true);
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        foreach (string value in Values)
            yield return value;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return (this as IEnumerable<string>).GetEnumerator();
    }

    public static bool operator ==(HadithReference first, HadithReference second)
    {
        if (Object.ReferenceEquals(first, null) && Object.ReferenceEquals(second, null))
            return true;
        if (Object.ReferenceEquals(first, null) || Object.ReferenceEquals(second, null))
            return false;
        return first.CompareTo(second) == 0;
    }

    public static bool operator !=(HadithReference first, HadithReference second)
    {
        return !(first == second);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is HadithReference))
            return false;
        return ((HadithReference)obj).CompareTo(this) == 0;
    }

    public override int GetHashCode()
    {
        // Must stay consistent with Equals/CompareTo, which are case-insensitive on
        // Code and Suffix and treat numeric values (e.g. "5" and "05") as equal.
        var hash = new HashCode();
        hash.Add(Code ?? "", StringComparer.OrdinalIgnoreCase);
        foreach (string value in Values)
            hash.Add(int.TryParse(value, out int numeric) ? numeric.ToString() : value);
        hash.Add(Suffix ?? "", StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public static HadithReference ParseDottedReference(string code, string hadithNumber)
    {
        string[] values = hadithNumber.Split('.');
        string lastValue = values[values.Length - 1];
        string[] lastValueAndSuffix = lastValue.Split('-');
        string suffix;
        if (lastValueAndSuffix.Length == 1)
            suffix = null;
        else
        {
            suffix = lastValueAndSuffix[1];
            values[values.Length - 1] = lastValueAndSuffix[0];
        }
        var result = new HadithReference(code, values, suffix);
        return result;
    }

    public void Assign(HadithReference otherReference)
    {
        if (otherReference.Code != Code)
            throw new ArgumentException("Cannot assign a HadithReference from a different collection");
        Values = new string[otherReference.Values.Length];
        for (int index = 0; index < Values.Length; index++)
            Values[index] = otherReference.Values[index];
        Suffix = otherReference.Suffix;
    }
}
