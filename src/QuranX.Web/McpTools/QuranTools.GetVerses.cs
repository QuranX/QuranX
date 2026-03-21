#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace QuranX.Web.McpTools;

partial class QuranTools
{
    public const string GetVersesName = "get_verses";

    [McpServerTool(
        Name = GetVersesName,
        Title = "Get Quran verses",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets multiple verses by multiple Verse Range References.")]
    public GetVersesResult GetVerses(
        [Description("A collection of verse references.")]
        VerseRangeReference[] verseRangeReferences,

        [Description(
            $$"""
            An optional collection of translator codes.
            If empty then all translations will be returned.
            See {{GetAvailableTranslatorsName}} for valid translator codes.
            """)]
        string[] translatorCodes)
    {
        var translatorCodesSet = new HashSet<string>(translatorCodes ?? [], StringComparer.OrdinalIgnoreCase);

        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.VerseRangeReferences", JsonSerializer.Serialize(verseRangeReferences));
            if (translatorCodesSet.Any())
                activity.SetTag("mcp.tool.arg.TranslatorCodes", string.Join(",", translatorCodesSet));
        }

        IEnumerable<Verse> retrievedVerses = VerseRepository.GetVerses(verseRangeReferences);

        if (translatorCodesSet.Any())
        {
            retrievedVerses =
                retrievedVerses
                .Select(x =>
                    new Verse(
                        chapterNumber: x.ChapterNumber,
                        verseNumber: x.VerseNumber,
                        rootWordCount: x.RootWordCount,
                        hadithCount: x.HadithCount,
                        commentaryCount: x.CommentaryCount,
                        verseTexts: x.VerseTexts.Where(vt => translatorCodesSet.Contains(vt.TranslatorCode))
                    ));
        }

        List<ChapterAndVerseSelection> selections = [];

        foreach (VerseRangeReference verseRangeReference in verseRangeReferences)
        {
            IEnumerable<Verse> currentSelection =
                retrievedVerses.Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber));

            ChapterAndVerseSelection chapterAndSelection =
                new(
                    ChapterRepository.Get(verseRangeReference.Chapter),
                    currentSelection);

            selections.Add(chapterAndSelection);
        }

        return new GetVersesResult
        {
            RequestedVerses = verseRangeReferences,
            RequestedTranslatorCodes = translatorCodesSet.ToArray(),
            ChaptersAndVerses = selections
        };
    }

    public sealed class GetVersesResult
    {
        public required VerseRangeReference[] RequestedVerses { get; init; }
        public IReadOnlyList<string> RequestedTranslatorCodes { get; init; } = [];
        public IReadOnlyList<ChapterAndVerseSelection> ChaptersAndVerses { get; init; } = [];
    }
}

