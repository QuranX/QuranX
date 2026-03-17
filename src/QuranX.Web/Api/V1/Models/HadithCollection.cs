#nullable enable
using System.Collections.Generic;

namespace QuranX.Web.Api.V1.Models;

public class HadithCollection
{
    public string Code { get; }
    public string Name { get; }
    public int HadithCount { get; }
    public string PrimaryReferenceCode { get; }
    public IEnumerable<HadithReferenceDefinition> ReferenceDefinitions { get; }

    public HadithCollection(
        string code,
        string name,
        int hadithCount,
        string primaryReferenceCode,
        IEnumerable<HadithReferenceDefinition> referenceDefinitions)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(code);
        System.ArgumentException.ThrowIfNullOrWhiteSpace(name);
        System.ArgumentException.ThrowIfNullOrWhiteSpace(primaryReferenceCode);

        Code = code;
        Name = name;
        HadithCount = hadithCount;
        PrimaryReferenceCode = primaryReferenceCode;
        ReferenceDefinitions = referenceDefinitions ?? throw new System.ArgumentNullException(nameof(referenceDefinitions));
    }
}

