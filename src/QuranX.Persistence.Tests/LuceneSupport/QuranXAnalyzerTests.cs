using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using QuranX.Persistence.LuceneSupport;
using System.IO;

namespace QuranX.Persistence.Tests.LuceneSupport;

[Trait("Category", "Unit")]
public sealed class QuranXAnalyzerTests
{
    private static List<string> TokenStreamFor(string fieldName, string input)
    {
        using var analyzer = new QuranXAnalyzer();
        using TokenStream stream = analyzer.GetTokenStream(fieldName, new StringReader(input));
        var termAttr = stream.AddAttribute<ICharTermAttribute>();
        var tokens = new List<string>();
        stream.Reset();
        while (stream.IncrementToken())
        {
            tokens.Add(termAttr.ToString());
        }
        stream.End();
        return tokens;
    }

    [Fact]
    public void EnglishInput_LowercasesTokens()
    {
        var tokens = TokenStreamFor("body", "Guidance For Travelers");
        Assert.Contains("guidance", tokens);
        Assert.Contains("for", tokens);
        Assert.Contains("travelers", tokens);
    }

    [Fact]
    public void EmptyInput_ProducesNoTokens()
    {
        var tokens = TokenStreamFor("body", string.Empty);
        Assert.Empty(tokens);
    }

    [Fact]
    public void DigitInput_KeepsDigits()
    {
        var tokens = TokenStreamFor("body", "verse 123");
        Assert.Contains("123", tokens);
    }

    [Fact]
    public void ArabicInput_ProducesStemmedTokens()
    {
        var tokens = TokenStreamFor("body", "كتب");
        Assert.NotEmpty(tokens);
    }
}
