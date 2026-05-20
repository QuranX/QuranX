using QuranX.Persistence.Extensions;

namespace QuranX.Persistence.Tests.Extensions;

[Trait("Category", "Unit")]
public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("", null)]
    [InlineData("hello", "hello")]
    [InlineData(" ", " ")]
    public void AsNullIfEmpty_ReturnsNullForEmptyOnly(string? input, string? expected)
    {
        Assert.Equal(expected, input!.AsNullIfEmpty());
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("\t\n", null)]
    [InlineData("hello", "hello")]
    [InlineData("  trim  ", "  trim  ")]
    public void AsNullIfWhiteSpace_ReturnsNullForWhitespace(string? input, string? expected)
    {
        Assert.Equal(expected, input!.AsNullIfWhiteSpace());
    }
}
