#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class ArabicAnalysisTools
{
    public const string GetArabicRootWordAnalysisName = "get_arabic_root_word_analysis";

    [McpServerTool(
        Name = GetArabicRootWordAnalysisName,
        Title = "Get Arabic root word analysis",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Get all occurrences of an Arabic root word in the Quran, grouped by word type and form.")]
    public GetArabicRootWordAnalysisResult GetArabicRootWordAnalysis(
        [Description("The Arabic root word letters (e.g. كتب).")]
        string arabicRootWord)
    {
        string root = ArabicHelper.SubstituteAndOmit(arabicRootWord);
        string romanized = ArabicHelper.ArabicToLetterNames(root);

        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.ArabicRootWord", arabicRootWord);
            activity.SetTag("mcp.tool.arg.RomanizedRootWord", romanized);
        }

        IEnumerable<VerseAnalysis> verses =
            VerseAnalysisRepository
            .GetForRoot(root)
            .OrderBy(x => x.ChapterNumber)
            .ThenBy(x => x.VerseNumber);

        var occurrences = new List<(string Type, string Form, ArabicRootWordOccurrence Occurrence)>();

        foreach (VerseAnalysis verseAnalysis in verses)
        {
            foreach (VerseAnalysisWord word in verseAnalysis.Words)
            {
                IEnumerable<VerseAnalysisWordPart> matchingParts =
                    word.WordParts.Where(x => x.Root == root);

                foreach (VerseAnalysisWordPart wordPart in matchingParts)
                {
                    const int WordsBeforeAndAfter = 3;
                    int lower = Math.Max(1, word.WordNumber - WordsBeforeAndAfter);
                    int upper = Math.Min(verseAnalysis.Words.Count, word.WordNumber + WordsBeforeAndAfter);

                    var contextWords = new List<ContextWord>();
                    for (int index = lower - 1; index < upper; index++)
                    {
                        VerseAnalysisWord contextWord = verseAnalysis.Words[index];
                        contextWords.Add(new ContextWord
                        {
                            Buckwalter = contextWord.Buckwalter,
                            English = contextWord.English,
                            IsSelected = contextWord.WordNumber == word.WordNumber
                        });
                    }

                    occurrences.Add((
                        Type: wordPart.Description,
                        Form: wordPart.Form,
                        new ArabicRootWordOccurrence
                        {
                            VerseReference = new VerseReference(verseAnalysis.ChapterNumber, verseAnalysis.VerseNumber),
                            SelectedWordBuckwalter = word.Buckwalter,
                            SelectedWordEnglish = word.English,
                            WordPartDescription = wordPart.Description,
                            WordPartDecorators = wordPart.Decorators,
                            ContextWords = contextWords
                        }));
                }
            }
        }

        var romanNumerals = new List<string> {
            "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"
        };

        var wordTypes = occurrences
            .GroupBy(x => new { x.Type, x.Form })
            .OrderBy(x => romanNumerals.IndexOf(x.Key.Form))
            .GroupBy(x => x.Key.Type)
            .OrderBy(x => x.Key)
            .Select(typeGroup =>
                new WordTypeResult
                {
                    Type = typeGroup.Key,
                    WordForms = typeGroup.Select(formGroup =>
                        new WordFormResult
                        {
                            Form = formGroup.Key.Form,
                            Occurrences = formGroup.Select(x => x.Occurrence).ToList()
                        }).ToList()
                }
            )
            .ToList();

        return new GetArabicRootWordAnalysisResult
        {
            RequestedArabicRootWord = root,
            RomanizedArabicRootWord = romanized,
            TotalOccurrencesInQuran = occurrences.Count,
            WordTypes = wordTypes
        };
    }

    public sealed class GetArabicRootWordAnalysisResult
    {
        public required string RequestedArabicRootWord { get; init; }
        public required string RomanizedArabicRootWord { get; init; }
        public required int TotalOccurrencesInQuran { get; init; }
        public required IReadOnlyList<WordTypeResult> WordTypes { get; init; }
    }

    public sealed class WordTypeResult
    {
        public required string Type { get; init; }
        public required IReadOnlyList<WordFormResult> WordForms { get; init; }
    }

    public sealed class WordFormResult
    {
        public required string Form { get; init; }
        public required IReadOnlyList<ArabicRootWordOccurrence> Occurrences { get; init; }
    }

    public sealed class ArabicRootWordOccurrence
    {
        public required VerseReference VerseReference { get; init; }
        public required string SelectedWordBuckwalter { get; init; }
        public required string SelectedWordEnglish { get; init; }
        public required string WordPartDescription { get; init; }
        public required IReadOnlyList<string> WordPartDecorators { get; init; }
        public required IReadOnlyList<ContextWord> ContextWords { get; init; }
    }

    public sealed class ContextWord
    {
        public required string Buckwalter { get; init; }
        public required string English { get; init; }
        public required bool IsSelected { get; init; }
    }
}
