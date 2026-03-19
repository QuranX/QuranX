#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class SearchTools
{
    [McpServerTool(
        Name = "search",
        Title = "Searches Quran verses, commentaries, and hadiths",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Searches Quran verses, commentaries, and hadiths matching a query")]
    public SearchResult Search(
        [Description("The search query text")]
        string query,
        [Description("The scope of the search. Valid values: 'wholesite' (default, searches everything), 'quran' (Quran verses only), 'commentaries' (tafsirs only), 'hadiths' (hadiths only). Leave null or empty to search the whole site.")]
        string? context = null,
        [Description("A commentator code when context is 'commentaries', or a hadith collection code when context is 'hadiths'. Use get_available_commentators or get_available_hadith_collections to discover valid codes. Leave null to search everything within the context.")]
        string? subContext = null)
    {
        string searchContext = (context ?? "").Trim().ToUpperInvariant() switch
        {
            "QURAN" => SearchContexts.Quran,
            "COMMENTARIES" => SearchContexts.Commentaries,
            "HADITHS" => SearchContexts.Hadiths,
            _ => SearchContexts.WholeSite
        };

        bool badQuery = false;
        var verseReferences = new List<VerseReference>();
        var commentaryResults = new List<CommentarySearchResult>();
        var hadithReferences = new List<HadithReference>();

        try
        {
            IEnumerable<Persistence.Models.SearchResult> searchResults =
                SearchEngine.Search(
                    query,
                    searchContext,
                    subContext,
                    out _,
                    maxResults: int.MaxValue);

            foreach (Persistence.Models.SearchResult result in searchResults)
            {
                switch (result.Type)
                {
                    case nameof(Verse):
                        int chapterNumber = result.Document.GetStoredValue((Verse x) => x.ChapterNumber);
                        int verseNumber = result.Document.GetStoredValue((Verse x) => x.VerseNumber);
                        verseReferences.Add(new VerseReference(chapterNumber, verseNumber));
                        break;

                    case nameof(Commentary):
                        string commentatorCode = result.Document.GetStoredValue((Commentary x) => x.CommentatorCode);
                        int commentaryChapter = result.Document.GetStoredValue((Commentary x) => x.ChapterNumber);
                        int commentaryVerse = result.Document.GetStoredValue((Commentary x) => x.FirstVerseNumber);
                        commentaryResults.Add(new CommentarySearchResult
                        {
                            CommentatorCode = commentatorCode,
                            VerseReference = new VerseReference(commentaryChapter, commentaryVerse)
                        });
                        break;

                    case nameof(Hadith):
                        Hadith hadith = result.Document.GetObject<Hadith>();
                        HadithReference? primaryReference = hadith.References
                            .FirstOrDefault(x => string.Compare(x.ReferenceCode, hadith.PrimaryReferenceCode, true) == 0);
                        if (primaryReference is not null)
                            hadithReferences.Add(primaryReference);
                        break;
                }
            }
        }
        catch (Lucene.Net.QueryParsers.Classic.ParseException)
        {
            badQuery = true;
        }

        return new SearchResult
        {
            RequestQuery = query,
            RequestContext = context,
            RequestSubContext = subContext,
            BadQuery = badQuery,
            VerseReferences = verseReferences.Distinct().ToArray(),
            Commentaries = commentaryResults
                .GroupBy(x => (x.CommentatorCode, x.VerseReference))
                .Select(g => g.First())
                .ToArray(),
            HadithReferences = hadithReferences.Distinct().ToArray()
        };
    }

    public sealed class SearchResult
    {
        public required string RequestQuery { get; init; }
        public required string? RequestContext { get; init; }
        public required string? RequestSubContext { get; init; }
        public required bool BadQuery { get; init; }
        public required VerseReference[] VerseReferences { get; init; }
        public required CommentarySearchResult[] Commentaries { get; init; }
        public required HadithReference[] HadithReferences { get; init; }
    }

    public sealed class CommentarySearchResult
    {
        public required string CommentatorCode { get; init; }
        public required VerseReference VerseReference { get; init; }
    }
}
