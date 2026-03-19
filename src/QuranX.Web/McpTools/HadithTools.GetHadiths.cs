#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    [McpServerTool(
        Name = "get_hadiths",
        Title = "Gets a specific hadith by reference",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets a specific hadith by its collection, reference code, and reference values")]
    public GetHadithsResult GetHadiths(
        [Description("The hadith collection code, e.g. 'bukhari', 'muslim'")]
        string collectionCode,

        [Description("The reference system code, e.g. 'inbook', 'uscmsa'")]
        string referenceCode,

        [Description("The first reference value, e.g. the book number")]
        int referenceValue1,

        [Description("The second reference value, e.g. the hadith number. Omit if the reference system has only one part.")]
        int? referenceValue2 = null,

        [Description("The third reference value. Omit if the reference system has fewer than three parts.")]
        int? referenceValue3 = null,

        [Description("An optional letter suffix on the final reference value, e.g. 'a' or 'b'")]
        string? suffix = null)
    {
        var reference = new HadithReference(
            collectionCode: collectionCode,
            referenceCode: referenceCode,
            referenceValue1: referenceValue1,
            referenceValue2: referenceValue2,
            referenceValue3: referenceValue3,
            suffix: suffix,
            primaryReferencePath: null);

        var hadiths = HadithRepository.GetHadiths([reference]);
        return new GetHadithsResult
        {
            RequestedHadithReference = reference,
            Hadiths = hadiths.ToArray()
        };
    }

    public sealed class GetHadithsResult
    {
        public required HadithReference RequestedHadithReference { get; init; }
        public required Hadith[] Hadiths { get; init; }
    }
}
