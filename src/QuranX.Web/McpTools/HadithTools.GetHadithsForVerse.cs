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
        $$"""
        Returns hadiths LINKED by citation to a specific Quran verse - i.e. hadiths that
        cite or relate to the verse - distinct from hadiths whose own narration text
        matches a keyword. Call this AFTER
        {{SearchTools.SearchName}}(context={{nameof(SearchTools.SearchContext.Quran)}})
        returns {{nameof(SearchTools.SearchResult.VerseReferences)}} (one call per verse)
        when the user wants to know what the Sunnah says about specific Quranic verses.
        Also useful after the user names verses, or after
        {{ArabicAnalysisTools.GetArabicRootWordAnalysisName}} to find hadiths citing
        root-occurrence verses.

        Each returned hadith carries {{nameof(Hadith.VerseRangeReferences)}} - all the
        verses it cites (often more than the one you queried for). Use these to fan out
        to {{QuranTools.GetVersesName}} or {{CommentaryTools.GetCommentariesForVerseName}}
        on the additional verses without re-running search.

        For breadth on a topic, run BOTH paths:
        {{SearchTools.SearchName}}(context={{nameof(SearchTools.SearchContext.Quran)}})
        -> {{GetHadithsForVerseName}} per verse, AND
        {{SearchTools.SearchName}}(context={{nameof(SearchTools.SearchContext.Hadiths)}})
        -> {{GetHadithsName}}.

        Optional hadithCollectionCodes filter restricts to specific collections (see
        {{GetAvailableHadithCollectionsName}}).
        """)]
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
