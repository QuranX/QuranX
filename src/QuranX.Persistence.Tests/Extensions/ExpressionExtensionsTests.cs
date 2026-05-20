using QuranX.Persistence.Extensions;
using System.Linq.Expressions;
using System;

namespace QuranX.Persistence.Tests.Extensions;

[Trait("Category", "Unit")]
public sealed class ExpressionExtensionsTests
{
    private sealed class Sample
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int? Score { get; set; }
    }

    [Fact]
    public void GetIndexName_ReturnsTypeNameUnderscorePropertyName()
    {
        string name = ExpressionExtensions.GetIndexName<Sample, string>(x => x.Name);
        Assert.Equal("Sample_Name", name);
    }

    [Fact]
    public void GetIndexName_IntProperty_ReturnsTypeNameUnderscorePropertyName()
    {
        string name = ExpressionExtensions.GetIndexName<Sample, int>(x => x.Age);
        Assert.Equal("Sample_Age", name);
    }

    [Fact]
    public void GetIndexName_NullableIntProperty_ReturnsTypeNameUnderscorePropertyName()
    {
        string name = ExpressionExtensions.GetIndexName<Sample, int?>(x => x.Score);
        Assert.Equal("Sample_Score", name);
    }

    [Fact]
    public void GetIndexNameAndPropertyValue_ReturnsBothNameAndValue()
    {
        var instance = new Sample { Name = "abc", Age = 42 };
        Expression<Func<Sample, string>> expression = x => x.Name;
        expression.GetIndexNameAndPropertyValue(
            instance,
            out string name,
            out string value);
        Assert.Equal("Sample_Name", name);
        Assert.Equal("abc", value);
    }

    [Fact]
    public void GetIndexNameAndPropertyValue_CompilesExpressionOncePerProperty()
    {
        var first = new Sample { Age = 1 };
        var second = new Sample { Age = 2 };
        Expression<Func<Sample, int>> expression = x => x.Age;

        expression.GetIndexNameAndPropertyValue(
            first,
            out _,
            out int firstValue);
        expression.GetIndexNameAndPropertyValue(
            second,
            out _,
            out int secondValue);

        Assert.Equal(1, firstValue);
        Assert.Equal(2, secondValue);
    }
}
