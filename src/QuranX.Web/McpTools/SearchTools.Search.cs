#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class SearchTools
{
    public enum SearchContext
    {
        [Description("Search everything")]
        WholeSite,

        [Description("Search the Quran only")]
        Quran,

        [Description("Search commentaries (tafsirs). Use subContext to specify a commentator code or leave null to search all.")]
        Commentaries,

        [Description("Search hadiths. Use subContext to specify a hadith collection code or leave null to search all.")]
        Hadiths
    }

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
        [Description("The search query text - this is a Lucene search engine query")]
        string luceneSearchQuery,
        [Description("The scope of the search")]
        SearchContext context = SearchContext.WholeSite,
        [Description("A commentator code when context is Commentaries, or a hadith collection code when context is Hadiths. Use get_available_commentators or get_available_hadith_collections to discover valid codes. Leave null to search everything within Context.")]
        string? subContext = null)
    {
        string searchContext = context switch
        {
            SearchContext.Quran => SearchContexts.Quran,
            SearchContext.Commentaries => SearchContexts.Commentaries,
            SearchContext.Hadiths => SearchContexts.Hadiths,
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
                    luceneSearchQuery,
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
            RequestQuery = luceneSearchQuery,
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
        public required SearchContext RequestContext { get; init; }
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
