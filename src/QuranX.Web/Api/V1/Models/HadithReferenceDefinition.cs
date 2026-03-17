#nullable enable
using System.Collections.Generic;

namespace QuranX.Web.Api.V1.Models;

public class HadithReferenceDefinition
{
    public string Code { get; }
    public string Name { get; }
    public string ValuePrefix { get; }
    public IEnumerable<string> PartNames { get; }

    public HadithReferenceDefinition(
        string code,
        string name,
        string valuePrefix,
        IEnumerable<string> partNames)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(code);
        System.ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name;
        ValuePrefix = valuePrefix;
        PartNames = partNames ?? throw new System.ArgumentNullException(nameof(partNames));
    }
}
