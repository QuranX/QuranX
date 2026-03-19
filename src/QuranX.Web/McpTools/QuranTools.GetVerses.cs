#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class QuranTools
{
    [McpServerTool(
        Name = "get_verses",
        Title = "Get Quran verses",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets Quran verses with translations")]
    public GetVersesResult GetVerses(
        [Description("A collection of verse range references in the format 'chapter.verse' or 'chapter.firstVerse-lastVerse', e.g. ['1.1', '2.255', '3.45-47']")]
        string[] verseRangeReferenceStrings,

        [Description("Optional collection of translator codes. Use get_available_translators to discover valid codes. If null or empty then all translations will be returned.")]
        string[]? translatorCodes = null)
    {
        VerseRangeReference[] verseRangeReferences = verseRangeReferenceStrings
            .Select(VerseRangeReference.Parse)
            .ToArray();
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

