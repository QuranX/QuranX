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

partial class CommentaryTools
{
    public const string GetCommentariesForVerseName = "get_commentaries_for_quran_verses";

    [McpServerTool(
        Name = GetCommentariesForVerseName,
        Title = "Get commentaries (aka tafsirs) for Quran verses",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Fetches the full text of classical commentaries (tafsirs) on a specific Quran
        verse. Call this AFTER {{SearchTools.SearchName}} returns
        {{nameof(SearchTools.SearchResult.VerseReferences)}} (run once per
        {{nameof(VerseReference)}}), or when the user names a specific verse to get
        scholarly commentary on it. Optionally restrict to one or more commentators via
        commentatorCodes (see {{GetAvailableCommentatorsName}}).

        Each returned commentary is anchored to a verse range via
        {{nameof(Commentary.ChapterNumber)}}, {{nameof(Commentary.FirstVerseNumber)}},
        and {{nameof(Commentary.LastVerseNumber)}} - useful when commentaries arrived via
        {{SearchTools.SearchName}} and you need the verse coordinates to fetch
        surrounding context with {{QuranTools.GetVersesName}}.

        This delivers tafsir CONTENT. Pair with {{HadithTools.GetHadithsForVerseName}}
        for hadiths linked to the same verse, or {{QuranTools.GetVersesName}} to read the
        verse text alongside.
        """)]
    public GetCommentariesForVersesResult GetCommentariesForVerses(
        [Description("Chapter and verse to retrieve commentaries/tafsirs for.")]
        VerseReference verseReference,

        [Description(
            $$"""
            Optional collection of commentator codes.
            If empty then all commentaries/tafsirs by all commentators will be returned
            for the specified verse.
            See {{GetAvailableCommentatorsName}} for valid commentator codes.
            """)]
        string[] commentatorCodes)
    {

        var commentatorCodesSet = new HashSet<string>(commentatorCodes ?? [], StringComparer.OrdinalIgnoreCase);
        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.VerseReference", verseReference);
            if (commentatorCodesSet.Any())
                activity.SetTag("mcp.tool.arg.CommentatorCodes", string.Join(",", commentatorCodesSet));
        }

        IEnumerable<Commentary> commentaries = CommentaryRepository.GetForVerse(verseReference.Chapter, verseReference.Verse);
        if (commentatorCodesSet.Any())
        {
            commentaries = commentaries.Where(x => commentatorCodesSet.Contains(x.CommentatorCode));
        }

        return new GetCommentariesForVersesResult
        {
            RequestedVerse = verseReference,
            RequestedCommentatorCodes = commentatorCodes,
            Commentaries = commentaries.ToArray()
        };
    }

    public sealed class GetCommentariesForVersesResult
    {
        public required VerseReference RequestedVerse { get; init; }
        public required string[]? RequestedCommentatorCodes { get; init; }
        public required Commentary[] Commentaries { get; init; }
    }
}


