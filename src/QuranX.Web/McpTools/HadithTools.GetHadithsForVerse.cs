#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    [McpServerTool(
        Name = "get_hadiths_for_verse",
        Title = "Gets all hadiths for a Quran verse",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets all hadiths for a Quran verse")]
    public GetHadithsForVerseResult GetHadithsForVerse(
        [Description("Verse reference in the format 'chapter.verse', e.g. '2.255'")]
        string verseReference,

        [Description("Optional collection of hadith collection codes. If null or empty then hadiths from all collections will be returned for the specified verse.")]
        string[]? hadithCollectionCodes = null)
    {
        VerseReference parsedVerse = VerseReference.Parse(verseReference);
        var hadithCollectionCodesSet = new HashSet<string>(hadithCollectionCodes ?? [], StringComparer.OrdinalIgnoreCase);

        IEnumerable<Hadith> hadiths = HadithRepository.GetForVerse(parsedVerse);
        if (hadithCollectionCodesSet.Any())
            hadiths = hadiths.Where(x => hadithCollectionCodesSet.Contains(x.CollectionCode));

        return new GetHadithsForVerseResult
        {
            RequestedVerse = parsedVerse,
            RequestedHadithCollectionCodes = hadithCollectionCodes,
            Hadiths = hadiths.ToArray()
        };
    }

    public sealed class GetHadithsForVerseResult
    {
        public required VerseReference RequestedVerse{ get; init; }
        public required string[]? RequestedHadithCollectionCodes { get; init; }
        public required Hadith[] Hadiths { get; init; }
    }
}
