using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuranX.Persistence.Models;

public class HadithReference : IComparable<HadithReference>
{
    [Description("The hadith collection code.")]
    public string CollectionCode { get; set;}

    [Description($"The index code by which to look up the hadith (aka {nameof(HadithReferenceDefinition)}).")]
    public string ReferenceCode { get; set;}

    [Description($"The first value of the index. The meaning of this value is defined in the {nameof(HadithReferenceDefinition.PartNames)} of the {nameof(HadithReferenceDefinition)} defined by {nameof(ReferenceCode)}.")]
    public int ReferenceValue1 { get; set;}

    [Description("The second value of the index.")]
    public int? ReferenceValue2 { get; set;}

    [Description("The third value of the index.")]
    public int? ReferenceValue3 { get; set;}

    [Description("If more than one hadith shares the same index, this property acts as a discriminator.")]
    public string Suffix { get; set;}

    [Description("Human readable string representing the index values.")]
    public string PrimaryReferencePath { get; set; }

    public HadithReference() { }

    public HadithReference(
        string collectionCode,
        string referenceCode,
        int referenceValue1,
        int? referenceValue2,
        int? referenceValue3,
        string suffix,
        string primaryReferencePath)
    {
        CollectionCode = collectionCode;
        ReferenceCode = referenceCode;
        ReferenceValue1 = referenceValue1;
        ReferenceValue2 = referenceValue2;
        ReferenceValue3 = referenceValue3;
        Suffix = suffix;
        PrimaryReferencePath = primaryReferencePath;
    }

    public IEnumerable<int> GetValues()
    {
        yield return ReferenceValue1;
        if (ReferenceValue2 is not null)
            yield return ReferenceValue2.Value;
        if (ReferenceValue3 is not null)
            yield return ReferenceValue3.Value;
    }

    public IEnumerable<KeyValuePair<string, string>> ToNameValuePairs(HadithReferenceDefinition definition)
    {
        if (definition is null)
            throw new ArgumentNullException(nameof(definition));
        if (string.Compare(definition.CollectionCode, CollectionCode, true) != 0)
            throw new ArgumentException(
                $"Hadith reference collection {CollectionCode} is not same as definition " +
                $"collection {definition.CollectionCode}",
                nameof(definition));
        if (string.Compare(definition.Code, ReferenceCode, true) != 0)
            throw new ArgumentException(
                $"Hadith reference code {ReferenceCode} is not same as definition " +
                $"code {definition.Code}",
                nameof(definition));
        int?[] values = [ReferenceValue1, ReferenceValue2, ReferenceValue3];
        return definition.PartNames.Select((partName, i) =>
        {
            string valueStr = values[i].ToString();
            if (i == definition.PartNames.Count - 1)
                valueStr += Suffix;
            return new KeyValuePair<string, string>(partName, valueStr);
        });
    }

    public static (int value, string suffix) SplitValue(string value)
    {
        var regex = new Regex(@"^(\d+)(\w+)?$");
        Match match = regex.Match(value);
        if (!match.Success)
            throw new ArgumentException("Must be digits alone or digits + letters", nameof(value));
        int val = int.Parse(match.Groups[1].Value);
        string suffix = match.Groups[2].Value;
        return (value: val, suffix);
    }

    public static bool TrySplitNameAndValue(string value, out (string referencePartName, int value, string suffix) result)
    {
        var regex = new Regex(@"^([a-zA-Z]+)-(\d+)(\w+)?$");
        Match match = regex.Match(value);
        if (!match.Success)
        {
            result = (referencePartName: null, value: 0, suffix: null);
            return false;
        }
        string referencePartName = match.Groups[1].Value;
        int val = int.Parse(match.Groups[2].Value);
        string suffix = match.Groups[3].Value;
        result = (referencePartName, value: val, suffix);
        return true;
    }

    public static (string referencePartName, int value, string suffix) SplitNameAndValue(string value)
    {
        (string referencePartName, int value, string suffix) result;
        if (!TrySplitNameAndValue(value, out result))
            throw new ArgumentException("Must be Name, a dash, and then digits alone or digits + letters", nameof(value));
        return result;
    }

    public override int GetHashCode()
    {
        // Must stay consistent with Equals, which is case-insensitive on
        // CollectionCode, ReferenceCode and Suffix.
        var hash = new HashCode();
        hash.Add(CollectionCode ?? "", StringComparer.OrdinalIgnoreCase);
        hash.Add(ReferenceCode ?? "", StringComparer.OrdinalIgnoreCase);
        hash.Add(ReferenceValue1);
        hash.Add(ReferenceValue2);
        hash.Add(ReferenceValue3);
        hash.Add(Suffix ?? "", StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public override bool Equals(object obj)
    {
        HadithReference other = obj as HadithReference;
        if (other is null)
            return false;

        return
            string.Compare(CollectionCode, other.CollectionCode, true) == 0
            && string.Compare(ReferenceCode, other.ReferenceCode, true) == 0
            && ReferenceValue1 == other.ReferenceValue1
            && ReferenceValue2 == other.ReferenceValue2
            && ReferenceValue3 == other.ReferenceValue3
            && string.Compare(Suffix, other.Suffix, true) == 0;
    }

    public int CompareTo(HadithReference other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));
        int result;
        if ((result = string.Compare(CollectionCode, other.CollectionCode, true)) != 0)
            return result;
        if ((result = string.Compare(ReferenceCode, other.ReferenceCode, true)) != 0)
            return result;
        if ((result = ReferenceValue1 - other.ReferenceValue1) != 0)
            return result;
        if ((result = (ReferenceValue2 ?? 0) - (other.ReferenceValue2 ?? 0)) != 0)
            return result;
        if ((result = (ReferenceValue3 ?? 0) - (other.ReferenceValue3 ?? 0)) != 0)
            return result;
        if ((result = (Suffix ?? "").Length - (other.Suffix ?? "").Length) != 0)
            return result;
        if ((result = string.Compare(Suffix, other.Suffix, true)) != 0)
            return result;
        return 0;
    }

    public HadithReference ExcludingSuffix()
    {
        return new HadithReference(
            collectionCode: CollectionCode,
            referenceCode: ReferenceCode,
            referenceValue1: ReferenceValue1,
            referenceValue2: ReferenceValue2,
            referenceValue3: ReferenceValue3,
            suffix: null,
            primaryReferencePath: null);
    }

    public string GetPath(HadithReferenceDefinition hadithReferenceDefinition)
    {
        var parts = new List<string>(3);
        int?[] values = [ReferenceValue1, ReferenceValue2, ReferenceValue3];
        int index = -1;
        foreach (string partName in hadithReferenceDefinition.PartNames)
        {
            index++;
            string valueStr = values[index].ToString();
            if (index == hadithReferenceDefinition.PartNames.Count - 1)
                valueStr += Suffix;
            parts.Add($"{partName}-{valueStr}");
        }
        string result = string.Join('/', parts);
        return $"{hadithReferenceDefinition.CollectionCode}/{hadithReferenceDefinition.Code}/{result}";
    }

    public static bool operator ==(HadithReference a, HadithReference b)
    {
        if (Object.ReferenceEquals(a, null))
            return Object.ReferenceEquals(b, null);
        return a.Equals(b);
    }

    public static bool operator !=(HadithReference a, HadithReference b)
    {
        return !(a == b);
    }
}
