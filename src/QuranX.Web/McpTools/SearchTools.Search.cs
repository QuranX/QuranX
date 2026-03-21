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
    [Description("Search Quran verses, commentaries, and hadiths.")]
    public SearchResult Search(
        [Description(
            $$"""
            The search query text. Supports all Lucene search engine syntax.
            Lucene examples:
                * To find a phrase, put consecutive words inside quotes, e.g. "The all knowing"
                * Spaces imply uppercase AND by default, e.g. benevolent merciful means benevolent AND merciful
                * To match either of several words, use uppercase OR, e.g. Muhammad OR Mohammed
                * To exclude a word, use uppercase NOT or prefix it with -, e.g. fight NOT kill or fight -kill
                * To control operator precedence, use parentheses, e.g. ((Muhammad OR Mohammed) love) OR Allah
                * Use ? as a single-character wildcard, e.g. M?hamm?d
                * Use * as a zero-or-more-character wildcard, e.g. Muham*
                * Do not use * or ? as the first character of a term
                * To do fuzzy matching, use ~, e.g. Muhammad~
                * To do phrase proximity search, use ~N after a quoted phrase, e.g. "all knowing"~3
                * To boost a term or phrase, use ^, e.g. Muhammad^2
                * To search a range, use [a TO b] for inclusive ranges or {a TO b}
                * To search for a literal special character, escape it with a backslash
            """)]
            
        string luceneSearchQuery,
        [Description("The scope of the search.")]
        SearchContext context = SearchContext.WholeSite,
        [Description(
            $$"""
            A commentator code when context is Commentaries, or a hadith collection
            code when context is Hadiths.
            Pass an empty string to search all subcontexts within the {{nameof(context)}}.
            * When {{nameof(context)}} is {{nameof(SearchContext.Hadiths)}} you can use
              {{HadithTools.GetAvailableHadithCollectionsName}} for a list of valid codes.
            * When {{nameof(context)}} is {{nameof(SearchContext.Commentaries)}} you can use
              {{CommentaryTools.GetAvailableCommentatorsName}} for a list of valid codes.
            * Otherwise it should be empty.
            """)]
        string subContext = "")
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
