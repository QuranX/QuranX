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
    [Description(
        $$"""
        Fetches Quran verse content - Arabic text, transliteration, and one or more
        English translations - for one or more verse ranges. This is what to call AFTER
        {{SearchTools.SearchName}} returns
        {{nameof(SearchTools.SearchResult.VerseReferences)}}, or when the user names
        specific chapter/verse coordinates.

        Each returned verse includes {{nameof(Verse.HadithCount)}} and
        {{nameof(Verse.CommentaryCount)}} - when either is greater than zero, the verse
        has linked hadiths or tafsirs you can drill into with
        {{HadithTools.GetHadithsForVerseName}} or
        {{CommentaryTools.GetCommentariesForVerseName}}.

        For follow-up context on the same verses, also call:
        {{CommentaryTools.GetCommentariesForVerseName}} (tafsirs),
        {{HadithTools.GetHadithsForVerseName}} (hadiths linked by citation),
        {{ArabicAnalysisTools.GetVerseRootWordAnalysisName}} (word-by-word grammar and
        Arabic roots). Use {{GetAvailableTranslatorsName}} if you want to filter to
        specific translators.
        """)]
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
            ChaptersAndVerses = selections,
            NextSteps =
                $$"""
                For each returned verse, the {{nameof(Verse.HadithCount)}} and
                {{nameof(Verse.CommentaryCount)}} fields signal whether more content is
                available:
                - If {{nameof(Verse.HadithCount)}} > 0, call {{HadithTools.GetHadithsForVerseName}}
                  to read hadiths citing the verse.
                - If {{nameof(Verse.CommentaryCount)}} > 0, call {{CommentaryTools.GetCommentariesForVerseName}}
                  to read tafsirs on the verse.
                For word-by-word Arabic analysis (roots, grammar) of any returned verse,
                call {{ArabicAnalysisTools.GetVerseRootWordAnalysisName}}. Do not stop
                after this call if the user's question covers tafsirs, hadiths, or word
                meaning - drill in.
                """
        };
    }

    public sealed class GetVersesResult
    {
        public required VerseRangeReference[] RequestedVerses { get; init; }
        public IReadOnlyList<string> RequestedTranslatorCodes { get; init; } = [];
        public IReadOnlyList<ChapterAndVerseSelection> ChaptersAndVerses { get; init; } = [];

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }
    }
}

