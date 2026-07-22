using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class HadithReferenceTests
{
    private static HadithReference Ref(string code, params string[] values) =>
        new(code: code, values: values, suffix: null);

    [Fact]
    public void CompareTo_NumericParts_SortsNaturallyNotLexically()
    {
        // "2" must sort before "10" (natural), not after it (lexical).
        HadithReference two = Ref("USC", "2");
        HadithReference ten = Ref("USC", "10");

        Assert.True(two.CompareTo(ten) < 0);
        Assert.True(ten.CompareTo(two) > 0);
    }

    [Fact]
    public void CompareTo_IsAntisymmetric_ForMixedLengthNonNumericParts()
    {
        // Non-numeric parts of differing lengths exercise the pad-and-compare
        // branch. Ordering must be strictly antisymmetric (H11: the old code
        // padded 'right' to the already-mutated length of 'left').
        HadithReference a = Ref("USC", "9a");
        HadithReference b = Ref("USC", "10a");

        int forward = a.CompareTo(b);
        int backward = b.CompareTo(a);

        Assert.NotEqual(0, forward);
        Assert.Equal(-Math.Sign(forward), Math.Sign(backward));
    }

    [Fact]
    public void CompareTo_EqualReferences_ReturnsZero()
    {
        Assert.Equal(0, Ref("USC", "3", "7").CompareTo(Ref("USC", "3", "7")));
    }

    [Fact]
    public void CompareTo_OrdersByCodeFirst()
    {
        Assert.True(Ref("AAA", "10").CompareTo(Ref("BBB", "2")) < 0);
    }
}
