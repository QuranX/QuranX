using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    public const string GetHadithsName = "get_hadiths";

    [McpServerTool(
        Name = GetHadithsName,
        Title = "Gets specific hadiths from one or more collections",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Fetches the full text of specific hadiths - narration, chain of narrators, and the
        full reference set - given their {{nameof(HadithReference)}} values. Call this
        AFTER
        {{SearchTools.SearchName}}(context={{nameof(SearchTools.SearchContext.Hadiths)}})
        returns {{nameof(SearchTools.SearchResult.HadithReferences)}}, or when the user
        supplies a specific hadith reference. Do not invent reference values - they come
        from {{SearchTools.SearchName}}, {{GetHadithsForVerseName}}, or are constructed by
        the user from {{GetAvailableHadithCollectionsName}} schemas.

        Each returned hadith carries {{nameof(Hadith.VerseRangeReferences)}} - the verses
        it cites. Use these to fan out to {{QuranTools.GetVersesName}} (verse text) and
        {{CommentaryTools.GetCommentariesForVerseName}} (tafsirs on those verses) without
        re-running search.
        """)]
    public GetHadithsResult GetHadiths(
        [Description("One or more hadith references identifying which hadiths to return.")]
        HadithReference[] hadithReferences)
    {
        var hadiths = HadithRepository.GetHadiths(hadithReferences);
        return new GetHadithsResult {
            RequestedHadithReferences = hadithReferences,
            Hadiths = hadiths.ToArray(),
            NextSteps =
                $$"""
                Each returned hadith carries {{nameof(Hadith.VerseRangeReferences)}} - the
                verses it cites. To read those verses, call {{QuranTools.GetVersesName}}.
                For tafsirs on the cited verses, call
                {{CommentaryTools.GetCommentariesForVerseName}} per verse. To find other
                hadiths citing the same verses, call {{GetHadithsForVerseName}} per verse.
                Do not stop after this call if the user's question covers verse content,
                tafsirs, or related hadiths.
                """
        };
    }

    public sealed class GetHadithsResult
    {
        public required HadithReference[] RequestedHadithReferences { get; init; }
        public required Hadith[] Hadiths { get; init; }

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }
    }
}
