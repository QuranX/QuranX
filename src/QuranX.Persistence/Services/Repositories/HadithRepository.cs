using Lucene.Net.Documents;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories;

public interface IHadithRepository
{
    bool HasReferences(
        string collectionCode,
        string referenceCode,
        IEnumerable<int> values,
        string suffix);
    IEnumerable<HadithReference> GetAllReferences(string collectionCode);
    IEnumerable<HadithReference> GetReferences(
        string collectionCode,
        string referenceCode,
        IEnumerable<int> values,
        string suffix);
    IEnumerable<Hadith> GetHadiths(string collectionCode, IEnumerable<string> primaryReferencePaths);
    IEnumerable<Hadith> GetHadiths(IEnumerable<HadithReference> references);
    IEnumerable<Hadith> GetForVerse(VerseReference verseReference);
}

public class HadithRepository : IHadithRepository
{
    private readonly IHadithCollectionRepository HadithCollectionRepository;
    private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

    public HadithRepository(
        IHadithCollectionRepository hadithCollectionRepository,
        ILuceneIndexSearcherProvider indexSearcherProvider)
    {
        HadithCollectionRepository = hadithCollectionRepository;
        IndexSearcherProvider = indexSearcherProvider;
    }

    public IEnumerable<HadithReference> GetAllReferences(string collectionCode)
    {
        HadithCollection collection = HadithCollectionRepository.Get(ref collectionCode);

        var query = new BooleanQuery(disableCoord: true);
        query
            .FilterByType<HadithReference>()
            .AddStringEqualsQuery<HadithReference>(x => x.CollectionCode, collectionCode, Occur.MUST);

        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        // Bound the result window to the number of documents that actually exist rather than
        // int.MaxValue: Lucene sizes its HitQueue array to numHits, so int.MaxValue attempts a
        // multi-gigabyte allocation. MaxDoc is the true upper bound on matches; guard the empty
        // index case, where MaxDoc == 0 would make Search throw ("numHits must be > 0").
        int maxResults = Math.Max(1, searcher.IndexReader.MaxDoc);
        TopDocs docs = searcher.Search(query, maxResults);
        IEnumerable<int> documentIds = docs.ScoreDocs.Select(x => x.Doc);
        IEnumerable<HadithReference> references = documentIds
            .Select(x => searcher.Doc(x).GetObject<HadithReference>());
        return references;
    }

    public IEnumerable<HadithReference> GetReferences(
        string collectionCode,
        string referenceCode,
        IEnumerable<int> values,
        string suffix)
    {
        IEnumerable<int> docIds = GetReferencesIds(
            collectionCode: collectionCode,
            referenceCode: referenceCode,
            values: values,
            suffix: suffix);
        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        IEnumerable<HadithReference> references = docIds
            .Select(x => searcher.Doc(x).GetObject<HadithReference>());
        return references;
    }

    public bool HasReferences(
        string collectionCode,
        string referenceCode,
        IEnumerable<int> values,
        string suffix)
    {
        return GetReferencesIds(
            collectionCode: collectionCode,
            referenceCode: referenceCode,
            values: values,
            suffix: suffix).Any();
    }

    public IEnumerable<Hadith> GetHadiths(string collectionCode, IEnumerable<string> primaryReferencePaths)
    {
        string[] paths = primaryReferencePaths?.Distinct().ToArray();
        if (paths is null || paths.Length == 0)
            return Array.Empty<Hadith>();

        var query = new BooleanQuery(disableCoord: true);
        query
            .FilterByType<Hadith>()
            .AddStringEqualsQuery<Hadith>(x => x.CollectionCode, collectionCode, Occur.MUST);
        var pathQuery = new BooleanQuery(disableCoord: true);
        foreach (string path in paths)
        {
            pathQuery.AddStringEqualsQuery<Hadith>(x => x.PrimaryReferencePath, path, Occur.SHOULD);
        }
        query.Add(new BooleanClause(pathQuery, Occur.MUST));

        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        // numHits must bound the number of matching *documents*, not the number of distinct paths:
        // several hadiths can share a PrimaryReferencePath, so paths.Length silently truncated the
        // results. MaxDoc is the true upper bound; guard the empty-index case (MaxDoc == 0 throws).
        int maxResults = Math.Max(1, searcher.IndexReader.MaxDoc);
        TopDocs docs = searcher.Search(query, maxResults);
        return docs.ScoreDocs
            .Select(x => searcher.Doc(x.Doc).GetObject<Hadith>());
    }

    public IEnumerable<Hadith> GetHadiths(IEnumerable<HadithReference> references)
    {
        if (references is null || !references.Any())
            return Array.Empty<Hadith>();

        return references
            .SelectMany(x =>
                GetReferences(
                    collectionCode: x.CollectionCode,
                    referenceCode: x.ReferenceCode,
                    values: x.GetValues(),
                    suffix: x.Suffix))
            .GroupBy(x => x.CollectionCode, StringComparer.OrdinalIgnoreCase)
            .SelectMany(g => GetHadiths(g.Key, g.Select(x => x.PrimaryReferencePath)));
    }

    private IEnumerable<int> GetReferencesIds(
        string collectionCode,
        string referenceCode,
        IEnumerable<int> values,
        string suffix)
    {
        int[] valuesArray = values?.ToArray() ?? Array.Empty<int>();
        HadithCollection collection = HadithCollectionRepository.Get(ref collectionCode);
        HadithReferenceDefinition referenceDefinition = collection.GetReferenceDefinition(referenceCode);

        var query = new BooleanQuery(disableCoord: true);
        query
            .FilterByType<HadithReference>()
            .AddStringEqualsQuery<HadithReference>(x => x.CollectionCode, collectionCode, Occur.MUST)
            .AddStringEqualsQuery<HadithReference>(x => x.ReferenceCode, referenceCode.Replace("-", ""), Occur.MUST);

        if (valuesArray.Length > 0)
        {
            query.AddNumericRangeQuery<HadithReference>(x =>
                x.ReferenceValue1,
                valuesArray[0],
                valuesArray[0],
                Occur.MUST);
        }
        if (valuesArray.Length > 1)
        {
            query.AddNumericRangeQuery<HadithReference>(x =>
                x.ReferenceValue2,
                valuesArray[1],
                valuesArray[1],
                Occur.MUST);
        }
        if (valuesArray.Length > 2)
        {
            query.AddNumericRangeQuery<HadithReference>(x =>
                x.ReferenceValue3,
                valuesArray[2],
                valuesArray[2],
                Occur.MUST);
        }

        // Only filter on suffix when all reference parts are specified
        if (valuesArray.Length == referenceDefinition.PartNames.Count
            && !string.IsNullOrEmpty(suffix))
        {
            query.AddStringEqualsQuery<HadithReference>(x =>
                x.Suffix,
                suffix,
                Occur.MUST);
        }

        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        TopDocs docs = searcher.Search(query, 99000);
        IEnumerable<int> hadithReferences = docs.ScoreDocs.Select(x => x.Doc).ToArray();
        return hadithReferences;
    }

    public IEnumerable<Hadith> GetForVerse(VerseReference verseReference)
    {
        int verseIndexValue = verseReference.ToIndexValue();
        var query = new BooleanQuery(disableCoord: true)
            .FilterByType<HadithVerseLink>()
            .AddNumericRangeQuery<HadithVerseLink>(x => x.VerseId, verseIndexValue, verseIndexValue, Occur.MUST);
        IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
        TopDocs docs = searcher.Search(query, 99000);
        return docs.ScoreDocs
            .Select(x => searcher.Doc(x.Doc).GetObject<HadithVerseLink>())
            .GroupBy(x => x.CollectionCode, StringComparer.OrdinalIgnoreCase)
            .SelectMany(g => GetHadiths(g.Key, g.Select(x => x.PrimaryReferencePath)));
    }
}

internal class HadithVerseLink
{
    public string CollectionCode { get; }
    public string PrimaryReferencePath { get; }
    public int VerseId { get; }

    public HadithVerseLink(string collectionCode, string primaryReferencePath, int verseId)
    {
        CollectionCode = collectionCode;
        PrimaryReferencePath = primaryReferencePath;
        VerseId = verseId;
    }
}
