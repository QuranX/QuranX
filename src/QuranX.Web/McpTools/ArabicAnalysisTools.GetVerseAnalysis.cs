#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    [Description(
        $$"""
        FIRST CHOICE when the user names a specific verse (e.g. "in 4.34", "Al-Baqarah
        255") and asks about words, Arabic terms, the meaning of an English gloss,
        etymology, or "where else is X used". Scan each word's
        {{nameof(AnalysisWord.English)}} for the user's term, then pivot to
        {{GetArabicRootWordAnalysisName}} via the matching
        {{nameof(AnalysisWord.Parts)}}[].{{nameof(AnalysisWordPart.ArabicRoot)}}.

        Returns a word-by-word grammatical breakdown of a Quran verse: each word's
        Buckwalter transliteration, English gloss, and for each word-part the type, form
        (I-X), Arabic root (Arabic + romanized), and decorators.

        PIVOT TO ROOT USAGE: each
        {{nameof(AnalysisWord.Parts)}}[].{{nameof(AnalysisWordPart.ArabicRoot)}} value
        can be passed straight to {{GetArabicRootWordAnalysisName}}, which finds every
        other place in the Quran that root appears - the standard follow-up when the
        user asks about a word's meaning, etymology, or thematic reach. The same root
        value also feeds {{DictionaryTools.GetDictionaryEntriesName}} for lexicographer
        definitions.
        """)]
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

        if (Activity.Current is Activity activity)
            activity.SetTag("mcp.tool.arg.VerseReference", verseReference);

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

        [Description("Buckwalter ASCII transliteration of the Arabic word.")]
        public required string Buckwalter { get; init; }

        public required string English { get; init; }
        public required IReadOnlyList<AnalysisWordPart> Parts { get; init; }
    }

    public sealed class AnalysisWordPart
    {
        public required string Type { get; init; }
        public required string Description { get; init; }

        [Description("Verb form for verbs (Roman numerals I-X); null for non-verbs.")]
        public required string? Form { get; init; }

        [Description(
            $$"""
            Arabic root letters (e.g. كتب). Pass to {{GetArabicRootWordAnalysisName}} for
            every Quranic occurrence of the root, or to
            {{DictionaryTools.GetDictionaryEntriesName}} for lexical definitions.
            """)]
        public required string? ArabicRoot { get; init; }

        [Description(
            $$"""
            Latin-letter transliteration of ArabicRoot for display when Arabic script is
            unavailable.
            """)]
        public required string? RomanizedRoot { get; init; }

        [Description(
            $$"""
            Linguistic feature tags from the morphological analysis (e.g. person, gender,
            number, mood, voice).
            """)]
        public required IReadOnlyList<string> Decorators { get; init; }
    }
}
