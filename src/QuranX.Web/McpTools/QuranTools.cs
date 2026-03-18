#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

public sealed class GetVersesResult
{
    public required string RequestedVerses { get; init; }
    public IReadOnlyList<string> TranslatorCodes { get; init; } = [];
    public IReadOnlyList<ChapterAndVerseSelection> Selections { get; init; } = [];
}

[McpServerToolType]
public sealed class QuranTools
{
    private readonly IChapterRepository ChapterRepository;
    private readonly IVerseRepository VerseRepository;

    public QuranTools(IChapterRepository chapterRepository, IVerseRepository verseRepository)
    {
        ChapterRepository = chapterRepository;
        VerseRepository = verseRepository;
    }

    [McpServerTool(
        Name = "get_verses",
        Title = "Get Quran verses",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets verses by reference string, e.g. '1.1' or '2.255,2.256-2.257'")]
    public GetVersesResult GetVerses(
        [Description("Comma-separated verse refs like '1.1' or '2.1-5' or '1.1,2.1-5'")]
        string verses,

        [Description("An optional collection of translator codes. If null or empty then all translations will be returned.")]
        IEnumerable<string>? translatorCodes = null)
    {
        VerseRangeReference[] verseRangeReferences =
            verses
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => VerseRangeReference.Parse(x))
            .ToArray();

        HashSet<string> translatorCodesSet = new(translatorCodes ?? [], StringComparer.OrdinalIgnoreCase);

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
            RequestedVerses = verses,
            TranslatorCodes = translatorCodesSet.ToArray(),
            Selections = selections
        };
    }
}