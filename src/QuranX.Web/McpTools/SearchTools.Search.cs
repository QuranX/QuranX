#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class SearchTools
{
    public const string SearchName = "search";

    public enum SearchContext
    {
        [Description("Search everything.")]
        WholeSite,

        [Description("Search the Quran only.")]
        Quran,

        [Description("Search commentaries (tafsirs).")]
        Commentaries,

        [Description("Search hadiths.")]
        Hadiths
    }

    [McpServerTool(
        Name = SearchName,
        Title = "Search Quran verses, commentaries, and hadiths.",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        "Search Quran verses, commentaries, and hadiths. Returns up to 100 references " +
        "ranked by relevance; the full match count is in `TotalResults`. " +
        "Follow up to fetch content: get_verses (verse text), " +
        "get_commentaries_for_quran_verses (tafsirs of a verse), " +
        "get_hadiths_for_verse (hadiths linked to a verse), or " +
        "get_hadiths (specific hadiths by reference).")]
    public SearchResult Search(
        [Description(
            $$"""
            The Apache Lucene 4.8.0 QueryParser query string used to search indexed text.
            The default operator is `OR`: bare space-separated terms match documents containing any of the terms, with documents matching more terms ranked higher. Use `AND` when every term must co-occur, and quotes for exact phrases.

            Use uppercase Boolean operators: AND, OR, and NOT. Use parentheses when combining clauses so precedence is unambiguous.

            Supported syntax:
            - Single term: mercy
            - Exact phrase: "all knowing"
            - Topical search (any term matches): charity OR orphans OR poor
            - Required combination: benevolent AND merciful
            - Spelling variants: Muhammad OR Mohammed
            - Exclusion: fight NOT kill or fight -kill
            - Grouping: (Muhammad OR Mohammed) AND love
            - Single-character wildcard: M?hamm?d
            - Multiple-character wildcard: Muham*
            - Regular expression term: /[mb]oat/
            - Fuzzy term: Muhammad~
            - Fuzzy term with edit distance: Muhammad~1
            - Phrase proximity: "all knowing"~3
            - Boosting: Muhammad^2
            - Inclusive range: [a TO b]
            - Exclusive range: {a TO b}

            Rules:
            - Boolean operators must be uppercase.
            - Do not use * or ? as the first character of a search term.
            - Wildcards apply to single terms, not quoted phrases.
            - NOT cannot be used by itself with only one term.
            - Escape literal query syntax characters with a backslash.
            """)]
            
        string luceneSearchQuery,
        [Description("The scope of the search. Defaults to WholeSite (search everything).")]
        SearchContext context = SearchContext.WholeSite,
        [Description(
            $$"""
            Narrows the search within {{nameof(context)}}. Pass an empty string to skip this filter.
            * When {{nameof(context)}} is {{nameof(SearchContext.Hadiths)}}: a hadith collection code (use {{HadithTools.GetAvailableHadithCollectionsName}} for valid codes).
            * When {{nameof(context)}} is {{nameof(SearchContext.Commentaries)}}: a commentator code (use {{CommentaryTools.GetAvailableCommentatorsName}} for valid codes).
            * Otherwise pass an empty string
            """)]
        string subContext = "")
    {
        if (Activity.Current is Activity activity)
        {
            activity.SetTag("mcp.tool.arg.LuceneSearchQuery", luceneSearchQuery);
            activity.SetTag("mcp.tool.arg.Context", context.ToString());
            if (!string.IsNullOrEmpty(subContext))
                activity.SetTag("mcp.tool.arg.SubContext", subContext);
        }

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
        int totalResults = 0;

        try
        {
            IEnumerable<Persistence.Models.SearchResult> searchResults =
                SearchEngine.Search(
                    luceneSearchQuery,
                    searchContext,
                    subContext,
                    out totalResults);

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
            TotalResults = totalResults,
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
        public required int TotalResults { get; init; }
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
