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
        [Description(
            $$"""
            Search the literal text of every corpus (Quran, commentaries, hadiths). Does
            NOT return hadiths LINKED to matched verses by citation - for that, use
            {{nameof(Quran)}} context then {{HadithTools.GetHadithsForVerseName}} on each
            {{nameof(VerseReference)}}.
            """)]
        WholeSite,

        [Description(
            $$"""
            Search the Quran. Use when the user asks about verses on a topic. Returned
            {{nameof(SearchResult.VerseReferences)}} feed {{QuranTools.GetVersesName}}
            (text), {{CommentaryTools.GetCommentariesForVerseName}} (tafsirs per verse),
            and {{HadithTools.GetHadithsForVerseName}} (linked hadiths per verse).
            """)]
        Quran,

        [Description(
            $$"""
            Search classical commentaries (tafsirs). Use when looking for tafsirs that
            mention a term. Returned {{nameof(SearchResult.Commentaries)}}[] feed
            {{CommentaryTools.GetCommentariesForVerseName}}({{nameof(CommentarySearchResult.VerseReference)}}, [{{nameof(CommentarySearchResult.CommentatorCode)}}]).
            """)]
        Commentaries,

        [Description(
            $$"""
            Search the literal narration text of hadiths. Use when looking for hadiths
            whose own text matches a keyword. Returned
            {{nameof(SearchResult.HadithReferences)}} feed {{HadithTools.GetHadithsName}}.
            NOTE: this finds hadiths whose text matches; for hadiths LINKED to a Quran
            verse by citation, search {{nameof(Quran)}} context then call
            {{HadithTools.GetHadithsForVerseName}}.
            """)]
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
        $$"""
        Search Quran verses, commentaries, and hadiths.

        DO NOT use this when the user has already named a specific verse (e.g. "4.34",
        "Al-Baqarah 255") and is asking about words/meaning/roots inside it - use
        {{ArabicAnalysisTools.GetVerseRootWordAnalysisName}} instead. Search finds
        verses by TOPIC across the corpus, NOT specific words within a named verse. For
        other by-verse queries on a named verse, see also {{QuranTools.GetVersesName}},
        {{CommentaryTools.GetCommentariesForVerseName}}, and
        {{HadithTools.GetHadithsForVerseName}}.

        OUTPUT IS REFERENCES, NOT CONTENT. Returns up to 1000 ranked items across three
        arrays: {{nameof(SearchResult.VerseReferences)}} (chapter+verse coordinates),
        {{nameof(SearchResult.HadithReferences)}} (collection + reference codes), and
        {{nameof(SearchResult.Commentaries)}} (commentator code + verse reference).

        To deliver a useful answer you almost always need a follow-up content fetch:
        - {{nameof(SearchResult.VerseReferences)}} -> {{QuranTools.GetVersesName}} for
            verse text.
        - {{nameof(SearchResult.VerseReferences)}} ->
            {{CommentaryTools.GetCommentariesForVerseName}} (one call per verse) for
            tafsirs on those verses.
        - {{nameof(SearchResult.VerseReferences)}} ->
            {{HadithTools.GetHadithsForVerseName}} (one call per verse) for hadiths
            linked by citation.
        - {{nameof(SearchResult.HadithReferences)}} -> {{HadithTools.GetHadithsName}} for
            hadith text and narrator chains.
        - {{nameof(SearchResult.Commentaries)}}[] - each item has
            {{nameof(CommentarySearchResult.CommentatorCode)}} and
            {{nameof(CommentarySearchResult.VerseReference)}}; pass to
            {{CommentaryTools.GetCommentariesForVerseName}}({{nameof(CommentarySearchResult.VerseReference)}}, [{{nameof(CommentarySearchResult.CommentatorCode)}}])
            for the specific tafsirs.

        {{nameof(SearchResult.TotalResults)}} reports the true match count - warn the
        user when it exceeds the 1000 returned. Default operator is OR; 2-5 alternative
        terms typically work well, ANDing many terms usually returns zero. For
        comprehensive answers, run multiple follow-up fetchers on the same search
        results (e.g. for a topic, fetch verses + tafsirs + linked hadiths in parallel).
        """)]
    public SearchResult Search(
        [Description(
            $$"""
            The Apache Lucene 4.8.0 QueryParser query string used to search indexed text.
            The default operator is `OR`: bare space-separated terms match documents
            containing any of the terms, with documents matching more terms ranked
            higher. Use `AND` when every term must co-occur, and quotes for exact phrases.

            Use uppercase Boolean operators: AND, OR, and NOT. Use parentheses when
            combining clauses so precedence is unambiguous.

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
            Narrows the search within {{nameof(context)}}. Pass an empty string to skip
            this filter.
            * When {{nameof(context)}} is {{nameof(SearchContext.Hadiths)}}: a hadith
                collection code (use {{HadithTools.GetAvailableHadithCollectionsName}}
                for valid codes).
            * When {{nameof(context)}} is {{nameof(SearchContext.Commentaries)}}: a
                commentator code (use {{CommentaryTools.GetAvailableCommentatorsName}}
                for valid codes).
            * Otherwise pass an empty string
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
        int totalResults = 0;

        try
        {
            IEnumerable<Persistence.Models.SearchResult> searchResults =
                SearchEngine.Search(
                    luceneSearchQuery,
                    searchContext,
                    subContext,
                    out totalResults,
                    maxResults: 1000);

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
            HadithReferences = hadithReferences.Distinct().ToArray(),
            NextSteps =
                $$"""
                For each non-empty result array, call the matching content fetcher next:
                - {{nameof(SearchResult.VerseReferences)}} -> {{QuranTools.GetVersesName}}
                  for verse text; or {{CommentaryTools.GetCommentariesForVerseName}} per
                  verse for tafsirs; or {{HadithTools.GetHadithsForVerseName}} per verse
                  for linked hadiths.
                - {{nameof(SearchResult.HadithReferences)}} -> {{HadithTools.GetHadithsName}}
                  for hadith text and narrator chains.
                - {{nameof(SearchResult.Commentaries)}} -> {{CommentaryTools.GetCommentariesForVerseName}} per entry.
                If {{nameof(SearchResult.BadQuery)}} is true, fix the query syntax and retry.
                If {{nameof(SearchResult.TotalResults)}} exceeds the array lengths, results
                were truncated - tell the user.
                Do not stop at this search result alone if the user's question requires content.
                """
        };
    }

    public sealed class SearchResult
    {
        public required string RequestQuery { get; init; }
        public required SearchContext RequestContext { get; init; }
        public required string? RequestSubContext { get; init; }

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }

        [Description(
            $$"""
            True when the Lucene parser rejected the query - fix the syntax (uppercase
            Boolean operators, balanced parens, no leading wildcard) and retry.
            """)]
        public required bool BadQuery { get; init; }

        [Description(
            $$"""
            Total match count in the index; may exceed the 1000 references returned. Warn
            the user when results are truncated.
            """)]
        public required int TotalResults { get; init; }

        [Description(
            $$"""
            Verse references from text matches. Pass to {{QuranTools.GetVersesName}} for
            verse text, {{CommentaryTools.GetCommentariesForVerseName}} for tafsirs, or
            {{HadithTools.GetHadithsForVerseName}} for linked hadiths.
            """)]
        public required VerseReference[] VerseReferences { get; init; }

        [Description(
            $$"""
            Tafsirs whose text matched the query. Each entry has the commentator code
            and the verse the tafsir is on; pass to
            {{CommentaryTools.GetCommentariesForVerseName}}.
            """)]
        public required CommentarySearchResult[] Commentaries { get; init; }

        [Description(
            $$"""
            Hadiths whose own narration text matched the query. Pass to
            {{HadithTools.GetHadithsName}}.
            """)]
        public required HadithReference[] HadithReferences { get; init; }
    }

    public sealed class CommentarySearchResult
    {
        [Description(
            $$"""
            Code identifying the commentator (mufassir). See
            {{CommentaryTools.GetAvailableCommentatorsName}}.
            """)]
        public required string CommentatorCode { get; init; }

        [Description(
            $$"""
            Verse this tafsir is anchored to. Pass to
            {{CommentaryTools.GetCommentariesForVerseName}} to fetch the tafsir text.
            """)]
        public required VerseReference VerseReference { get; init; }
    }
}
