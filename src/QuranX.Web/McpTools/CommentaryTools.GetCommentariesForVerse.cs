
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
    [Description("Get commentaries (aka tafsirs) for a specific chapter and verse.")]
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


