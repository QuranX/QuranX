using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories;

public interface IDictionaryEntryRepository
{
    IEnumerable<DictionaryEntry> Get(string word);
    IEnumerable<DictionaryEntry> Get(string dictionaryCode, string word);
    IEnumerable<string> GetAll();
    IEnumerable<string> GetNextRoots(string parentRoot);
}

public class DictionaryEntryRepository : IDictionaryEntryRepository
{
    public const string RootWordsIndexName = nameof(DictionaryEntry) + "_WordIndex";
    private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

    public DictionaryEntryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
    {
        IndexSearcherProvider = indexSearcherProvider;
    }

    public IEnumerable<DictionaryEntry> Get(string word)
    {
        string indexValue = ArabicWordIndexer.GetIndexForArabic(word);
        var query = new BooleanQuery(disableCoord: true);
        query
            .FilterByType<DictionaryEntry>()
            .AddStringEqualsQuery(RootWordsIndexName, indexValue, Occur.MUST);

        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        TopDocs docs = searcher.Search(query, 7000);
        IEnumerable<DictionaryEntry> results = docs.ScoreDocs
            .Select(x => searcher.Doc(x.Doc))
            .Select(x => x.GetObject<DictionaryEntry>())
            .OrderBy(x => x.Word)
            .ToArray();
        return results;
    }

    public IEnumerable<DictionaryEntry> Get(string dictionaryCode, string word)
    {
        string indexValue = ArabicWordIndexer.GetIndexForArabic(word);
        var query = new BooleanQuery(disableCoord: true);
        query.FilterByType<DictionaryEntry>();
        query
            .AddStringEqualsQuery<DictionaryEntry>(x => x.DictionaryCode, dictionaryCode, Occur.MUST)
            .AddStringEqualsQuery(RootWordsIndexName, indexValue, Occur.MUST);

        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        TopDocs docs = searcher.Search(query, 7000);
        IEnumerable<DictionaryEntry> results = docs.ScoreDocs
            .Select(x => searcher.Doc(x.Doc))
            .Select(x => x.GetObject<DictionaryEntry>())
            .OrderBy(x => x.Word)
            .ToArray();
        return results;
    }

    public IEnumerable<string> GetAll()
    {
        var query = new BooleanQuery(disableCoord: true);
        query.FilterByType<DictionaryEntry>();
        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        // Bound the result window to the number of documents that actually exist rather than
        // int.MaxValue: Lucene sizes its HitQueue array to numHits, so int.MaxValue attempts a
        // multi-gigabyte allocation. MaxDoc is the true upper bound on matches; guard the empty
        // index case, where MaxDoc == 0 would make Search throw ("numHits must be > 0").
        int maxResults = Math.Max(1, searcher.IndexReader.MaxDoc);
        TopDocs docs = searcher.Search(query, maxResults);
        string[] roots = docs
            .ScoreDocs
            .Select(x => searcher.Doc(x.Doc))
            .Select(x => x.GetStoredValue<DictionaryEntry>(x => x.Word))
            .Distinct()
            .OrderBy(x => x)
            .ToArray();
        return roots;
    }

    public IEnumerable<string> GetNextRoots(string root)
    {
        string[] roots;
        if (string.IsNullOrWhiteSpace(root))
        {
            roots = ArabicAlphabet.Letters.Select(x => x.ToString()).ToArray();
        }
        else
        {
            var query = new BooleanQuery(disableCoord: true);
            query
                .FilterByType<DictionaryEntry>()
                .AddStringStartsWithQuery<DictionaryEntry>(x => x.Word, root, Occur.MUST);

            int nextRootLength = root.Length + 1;
            IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
            TopDocs docs = searcher.Search(query, 7000);
            roots = docs.ScoreDocs
                .Select(x => searcher.Doc(x.Doc))
                .Select(x => x.GetStoredValue<DictionaryEntry>(x => x.Word))
                .Where(x => x.Length >= nextRootLength)
                .Select(x => x.Substring(0, nextRootLength))
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }
        return roots;
    }
}
