
#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class CommentaryTools
{
    [McpServerTool(
        Name = "get_commentaries_for_quran_verses",
        Title = "Get commentaries (aka tafsirs) for Quran verses",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets commentaries (aka tafsirs) for a specific chapter and verse")]
    public GetCommentariesForVersesResult GetCommentariesForVerses(
        [Description("Verse reference in the format 'chapter.verse', e.g. '2.255'")]
        string verseReference,

        [Description("Optional collection of commentator codes. Use get_available_commentators to discover valid codes. If null or empty then commentaries/tafsirs by all commentators will be returned.")]
        string[]? commentatorCodes = null)
    {
        VerseReference parsedVerse = VerseReference.Parse(verseReference);
        var commentatorCodesSet = new HashSet<string>(commentatorCodes ?? [], StringComparer.OrdinalIgnoreCase);

        IEnumerable<Commentary> commentaries = CommentaryRepository.GetForVerse(parsedVerse.Chapter, parsedVerse.Verse);
        if (commentatorCodesSet.Any())
        {
            commentaries = commentaries.Where(x => commentatorCodesSet.Contains(x.CommentatorCode));
        }

        return new GetCommentariesForVersesResult
        {
            RequestedVerse = parsedVerse,
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


