#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    public const string GetHadithsForVerseName = "get_hadiths_for_verse";

    [McpServerTool(
        Name = GetHadithsForVerseName,
        Title = "Gets all hadiths for a Quran verse",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        "Returns hadiths that are linked to a specific Quran verse (i.e. hadiths that cite or relate to the verse) — " +
        "distinct from keyword-searching the hadith corpus. " +
        "Use after `search` returns verse references to find the hadiths associated with those verses.")]
    public GetHadithsForVerseResult GetHadithsForVerse(
        [Description("Chapter and verse to retrieve hadiths for.")]
        VerseReference verseReference,

        [Description(
            $$"""
            Optional collection of hadith collection codes.
            If empty then hadiths from all collections will be returned for the specified verse.
            See {{GetAvailableHadithCollectionsName}} for valid codes.
            """)]
        string[] hadithCollectionCodes)
    {
        var hadithCollectionCodesSet = new HashSet<string>(hadithCollectionCodes ?? [], StringComparer.OrdinalIgnoreCase);

        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.VerseReference", verseReference);
            if (hadithCollectionCodesSet.Any())
                activity.SetTag("mcp.tool.arg.HadithCollectionCodes", string.Join(",", hadithCollectionCodesSet));
        }

        IEnumerable<Hadith> hadiths = HadithRepository.GetForVerse(verseReference);
        if (hadithCollectionCodesSet.Any())
            hadiths = hadiths.Where(x => hadithCollectionCodesSet.Contains(x.CollectionCode));

        return new GetHadithsForVerseResult
        {
            RequestedVerse = verseReference,
            RequestedHadithCollectionCodes = hadithCollectionCodes,
            Hadiths = hadiths.ToArray()
        };
    }

    public sealed class GetHadithsForVerseResult
    {
        public required VerseReference RequestedVerse{ get; init; }
        public required string[]? RequestedHadithCollectionCodes { get; init; }
        public required Hadith[] Hadiths { get; init; }
    }
}
