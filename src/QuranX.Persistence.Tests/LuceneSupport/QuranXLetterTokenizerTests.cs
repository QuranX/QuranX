using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using QuranX.Persistence.LuceneSupport;
using System.IO;

namespace QuranX.Persistence.Tests.LuceneSupport;

[Trait("Category", "Unit")]
public sealed class QuranXLetterTokenizerTests
{
    private static List<string> Tokenize(string input)
    {
        using var tokenizer = new QuranXLetterTokenizer(new StringReader(input));
        var termAttr = tokenizer.AddAttribute<ICharTermAttribute>();
        var tokens = new List<string>();
        tokenizer.Reset();
        while (tokenizer.IncrementToken())
        {
            tokens.Add(termAttr.ToString());
        }
        tokenizer.End();
        return tokens;
    }

    [Fact]
    public void Letters_TokenizedAsSingleToken()
    {
        var tokens = Tokenize("hello");
        Assert.Single(tokens);
        Assert.Equal("hello", tokens[0]);
    }

    [Fact]
    public void Whitespace_SplitsTokens()
    {
        var tokens = Tokenize("hello world");
        Assert.Equal(2, tokens.Count);
    }

    [Fact]
    public void Digits_KeptInTokens()
    {
        var tokens = Tokenize("abc123 xyz");
        Assert.Contains("abc123", tokens);
        Assert.Contains("xyz", tokens);
    }

    [Fact]
    public void Punctuation_SplitsTokens()
    {
        var tokens = Tokenize("foo,bar");
        Assert.Equal(2, tokens.Count);
    }

    [Fact]
    public void ArabicLetters_TokenizedTogether()
    {
        var tokens = Tokenize("كتب");
        Assert.Single(tokens);
        Assert.Equal("كتب", tokens[0]);
    }
}
