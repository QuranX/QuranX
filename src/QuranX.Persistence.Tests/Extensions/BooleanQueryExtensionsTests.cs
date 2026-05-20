using Lucene.Net.Search;
using QuranX.Persistence.Extensions;

namespace QuranX.Persistence.Tests.Extensions;

[Trait("Category", "Unit")]
public sealed class BooleanQueryExtensionsTests
{
    private sealed class Item
    {
        public string CollectionCode { get; set; } = string.Empty;
        public int Value { get; set; }
        public int? OptionalValue { get; set; }
    }

    [Fact]
    public void FilterByType_AddsPhraseQueryWithMustOccur()
    {
        var query = new BooleanQuery();
        query.FilterByType<Item>();

        Assert.Single(query.Clauses);
        Assert.Equal(Occur.MUST, query.Clauses[0].Occur);
        Assert.IsType<PhraseQuery>(query.Clauses[0].Query);
    }

    [Fact]
    public void AddStringEqualsQuery_WithValue_AddsTermQuery()
    {
        var query = new BooleanQuery();
        query.AddStringEqualsQuery<Item>(x => x.CollectionCode, "Bukhari", Occur.MUST);

        Assert.Single(query.Clauses);
        Assert.IsType<TermQuery>(query.Clauses[0].Query);
        var termQuery = (TermQuery)query.Clauses[0].Query;
        Assert.Equal("Item_CollectionCode", termQuery.Term.Field);
        Assert.Equal("Bukhari", termQuery.Term.Text);
    }

    [Fact]
    public void AddStringEqualsQuery_NullValue_DoesNotAddClause()
    {
        var query = new BooleanQuery();
        query.AddStringEqualsQuery<Item>(x => x.CollectionCode, null!, Occur.MUST);
        Assert.Empty(query.Clauses);
    }

    [Fact]
    public void AddStringEqualsQuery_ByIndexName_AddsTermQuery()
    {
        var query = new BooleanQuery();
        query.AddStringEqualsQuery("CustomField", "value", Occur.SHOULD);

        Assert.Single(query.Clauses);
        Assert.Equal(Occur.SHOULD, query.Clauses[0].Occur);
        var termQuery = (TermQuery)query.Clauses[0].Query;
        Assert.Equal("CustomField", termQuery.Term.Field);
    }

    [Fact]
    public void AddStringStartsWithQuery_WithValue_AddsPrefixQuery()
    {
        var query = new BooleanQuery();
        query.AddStringStartsWithQuery<Item>(x => x.CollectionCode, "Buk", Occur.MUST);

        Assert.Single(query.Clauses);
        Assert.IsType<PrefixQuery>(query.Clauses[0].Query);
        var prefixQuery = (PrefixQuery)query.Clauses[0].Query;
        Assert.Equal("Item_CollectionCode", prefixQuery.Prefix.Field);
        Assert.Equal("Buk", prefixQuery.Prefix.Text);
    }

    [Fact]
    public void AddStringStartsWithQuery_NullValue_DoesNotAddClause()
    {
        var query = new BooleanQuery();
        query.AddStringStartsWithQuery<Item>(x => x.CollectionCode, null!, Occur.MUST);
        Assert.Empty(query.Clauses);
    }

    [Fact]
    public void AddNumericRangeQuery_Int_AddsNumericRangeQuery()
    {
        var query = new BooleanQuery();
        query.AddNumericRangeQuery<Item>(x => x.Value, 1, 5, Occur.MUST);

        Assert.Single(query.Clauses);
        Assert.IsType<NumericRangeQuery<int>>(query.Clauses[0].Query);
    }

    [Fact]
    public void AddNumericRangeQuery_NullableInt_AddsNumericRangeQuery()
    {
        var query = new BooleanQuery();
        query.AddNumericRangeQuery<Item>(x => x.OptionalValue, 10, 20, Occur.SHOULD);

        Assert.Single(query.Clauses);
        Assert.Equal(Occur.SHOULD, query.Clauses[0].Occur);
        Assert.IsType<NumericRangeQuery<int>>(query.Clauses[0].Query);
    }

    [Fact]
    public void FluentCalls_ChainMultipleClauses()
    {
        var query = new BooleanQuery(disableCoord: true);
        query
            .FilterByType<Item>()
            .AddStringEqualsQuery<Item>(x => x.CollectionCode, "Bukhari", Occur.MUST)
            .AddNumericRangeQuery<Item>(x => x.Value, 1, 1, Occur.MUST);

        Assert.Equal(3, query.Clauses.Count);
    }
}
