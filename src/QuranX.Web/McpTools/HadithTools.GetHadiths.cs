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
        [Description("The hadith collection code. Use get_available_hadith_collections to discover valid codes, e.g. 'bukhari', 'muslim', 'abudawud'.")]
        string collectionCode,

        [Description("The reference system code for the collection. Use get_available_hadith_collections to discover valid reference codes and their part names for each collection, e.g. 'inbook', 'uscmsa'.")]
        string referenceCode,

        [Description("The first reference part value (e.g. book number). The meaning depends on the reference system's part names returned by get_available_hadith_collections.")]
        int referenceValue1,

        [Description("The second reference part value (e.g. hadith number). Only provide if the reference system has two or more parts.")]
        int? referenceValue2 = null,

        [Description("The third reference part value. Only provide if the reference system has three parts.")]
        int? referenceValue3 = null,

        [Description("An optional letter suffix on the final reference value, e.g. 'a' or 'b'. Most hadiths do not have a suffix.")]
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
