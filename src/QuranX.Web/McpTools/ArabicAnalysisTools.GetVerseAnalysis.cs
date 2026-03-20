#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class ArabicAnalysisTools
{
    public const string GetVerseRootWordAnalysisName = "get_verse_root_word_analysis";

    [McpServerTool(
        Name = GetVerseRootWordAnalysisName,
        Title = "Get verse analysis",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Get the word-by-word grammatical analysis of a Quran verse, including roots, word types, forms, and decorators for each word part.")]
    public GetVerseAnalysisResult GetVerseAnalysis(
        [Description("The verse to analyse.")]
        VerseReference verseReference)
    {
        if (!QuranStructure.TryValidateChapterAndVerse(verseReference.Chapter, verseReference.Verse))
            return new GetVerseAnalysisResult
            {
                RequestedVerse = verseReference,
                Words = []
            };

        VerseAnalysis analysis =
            VerseAnalysisRepository.GetForVerse(verseReference.Chapter, verseReference.Verse);

        IEnumerable<AnalysisWord> words =
            analysis?
            .Words
            .Select(w =>
                new AnalysisWord
                {
                    WordNumber = w.WordNumber,
                    Buckwalter = w.Buckwalter,
                    English = w.English,
                    Parts = w.WordParts
                        .Select(p =>
                            new AnalysisWordPart
                            {
                                Type = p.Type,
                                Description = p.Description,
                                Form = string.IsNullOrEmpty(p.Form) ? null : p.Form,
                                ArabicRoot = string.IsNullOrEmpty(p.Root) ? null : p.Root,
                                RomanizedRoot = string.IsNullOrEmpty(p.Root) ? null : ArabicHelper.ArabicToLetterNames(p.Root),
                                Decorators = p.Decorators
                            }
                        )
                        .ToList()
                }
            )
            ?? [];

        return new GetVerseAnalysisResult
        {
            RequestedVerse = verseReference,
            Words = words.ToArray()
        };
    }

    public sealed class GetVerseAnalysisResult
    {
        public required VerseReference RequestedVerse { get; init; }
        public required AnalysisWord[] Words { get; init; }
    }

    public sealed class AnalysisWord
    {
        public required int WordNumber { get; init; }
        public required string Buckwalter { get; init; }
        public required string English { get; init; }
        public required IReadOnlyList<AnalysisWordPart> Parts { get; init; }
    }

    public sealed class AnalysisWordPart
    {
        public required string Type { get; init; }
        public required string Description { get; init; }
        public required string? Form { get; init; }
        public required string? ArabicRoot { get; init; }
        public required string? RomanizedRoot { get; init; }
        public required IReadOnlyList<string> Decorators { get; init; }
    }
}
