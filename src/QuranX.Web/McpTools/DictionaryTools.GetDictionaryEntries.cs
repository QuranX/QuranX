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

partial class DictionaryTools
{
    public const string GetDictionaryEntriesName = "get_dictionary_entries";

    [McpServerTool(
        Name = GetDictionaryEntriesName,
        Title = "Get dictionary entries",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Fetches Arabic dictionary entries for a root letter string (e.g. كتب). Returns
        one or more entries per dictionary as HTML.

        ENTRY POINTS: a root supplied by the user; from
        {{ArabicAnalysisTools.GetVerseRootWordAnalysisName}} via
        {{nameof(ArabicAnalysisTools.AnalysisWord.Parts)}}[]
        .{{nameof(ArabicAnalysisTools.AnalysisWordPart.ArabicRoot)}}; or from
        {{ArabicAnalysisTools.GetArabicRootWordAnalysisName}}.

        FOLLOW-UPS: pair with {{ArabicAnalysisTools.GetArabicRootWordAnalysisName}}(root)
        to show every Quranic occurrence of the same root alongside its lexical
        definitions - the standard combined response for "what does this Arabic word
        mean?".

        Optional dictionaryCode restricts to a specific dictionary; otherwise entries
        from every dictionary are returned. See {{GetAvailableDictionariesName}} for
        valid codes.
        """)]
    public GetDictionaryEntriesResult GetDictionaryEntries(
        [Description("The Arabic word root letters (e.g. كتب).")]
        string arabicWordRoot,

        [Description(
            $$"""
            An optional dictionary code to filter results to a specific dictionary.
            If empty, entries from all dictionaries are returned.
            See {{GetAvailableDictionariesName}} for valid dictionary codes.
            """)]
        string dictionaryCode = "")
    {
        string root = ArabicHelper.SubstituteAndOmit(arabicWordRoot);
        string romanized = ArabicHelper.ArabicToLetterNames(root);

        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.ArabicRootWord", arabicWordRoot);
            activity.SetTag("mcp.tool.arg.RomanizedRootWord", romanized);
            if (!string.IsNullOrEmpty(dictionaryCode))
                activity.SetTag("mcp.tool.arg.DictionaryCode", dictionaryCode);
        }


        IEnumerable<DictionaryEntry> entries =
            string.IsNullOrWhiteSpace(dictionaryCode)
            ? DictionaryEntryRepository.Get(root)
            : DictionaryEntryRepository.Get(dictionaryCode, root);

        var grouped = entries
            .OrderBy(x => x.DictionaryCode)
            .ThenBy(x => x.EntryIndex)
            .GroupBy(x => x.DictionaryCode)
            .Select(g =>
            {
                string code = g.Key;
                Dictionary? dictionary = DictionaryRepository.Get(ref code);
                return new DictionaryEntryGroup
                {
                    DictionaryCode = code,
                    DictionaryName = dictionary?.Name ?? code,
                    Entries = g.Select(e => new DictionaryEntryResult
                    {
                        EntryIndex = e.EntryIndex,
                        Html = e.Html
                    }).ToList()
                };
            })
            .ToList();

        return new GetDictionaryEntriesResult
        {
            ArabicWordRoot = root,
            RomanizedWordRoot = romanized,
            Dictionaries = grouped
        };
    }

    public sealed class GetDictionaryEntriesResult
    {
        public required string ArabicWordRoot { get; init; }
        public required string RomanizedWordRoot { get; init; }
        public required IReadOnlyList<DictionaryEntryGroup> Dictionaries { get; init; }
    }

    public sealed class DictionaryEntryGroup
    {
        public required string DictionaryCode { get; init; }
        public required string DictionaryName { get; init; }
        public required IReadOnlyList<DictionaryEntryResult> Entries { get; init; }
    }

    public sealed class DictionaryEntryResult
    {
        public required int EntryIndex { get; init; }
        public required IReadOnlyList<string> Html { get; init; }
    }
}
