using QuranX.DocumentModel;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class HadithComparerTests
{
    private static HadithReferenceDefinition Definition(string code, bool isPrimary) =>
        new(
            isPrimary: isPrimary,
            code: code,
            name: code,
            partNames: ["Number"]);

    private static HadithCollection Collection() => new(
        code: "Bukhari",
        name: "Sahih al-Bukhari",
        copyright: "",
        referenceDefinitions: [
            Definition("USC", isPrimary: true),
            Definition("InBook", isPrimary: false),
        ]);

    private static Hadith Build(HadithCollection collection, HadithReference reference) =>
        new(
            collection: collection,
            references: [reference],
            arabicText: ["arabic"],
            englishText: ["english"],
            verseReferences: System.Array.Empty<VerseRangeReference>());

    [Fact]
    public void Constructor_NullReferenceDefinitions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new HadithComparer(null!));
    }

    [Fact]
    public void Constructor_PutsPrimaryReferenceDefinitionFirst()
    {
        var comparer = new HadithComparer([
            Definition("InBook", isPrimary: false),
            Definition("USC", isPrimary: true),
        ]);

        Assert.Equal("USC", comparer.CollectionCodesInPriorityOrder[0]);
    }

    [Fact]
    public void Compare_NullFirst_Throws()
    {
        var collection = Collection();
        var comparer = new HadithComparer(collection.ReferenceDefinitions);
        var hadith = Build(collection, new HadithReference("USC", ["1"], null));

        Assert.Throws<ArgumentNullException>(() => comparer.Compare(null!, hadith));
    }

    [Fact]
    public void Compare_NullSecond_Throws()
    {
        var collection = Collection();
        var comparer = new HadithComparer(collection.ReferenceDefinitions);
        var hadith = Build(collection, new HadithReference("USC", ["1"], null));

        Assert.Throws<ArgumentNullException>(() => comparer.Compare(hadith, null!));
    }

    [Fact]
    public void Compare_SameReferenceType_ReturnsReferenceComparison()
    {
        var collection = Collection();
        var comparer = new HadithComparer(collection.ReferenceDefinitions);
        var first = Build(collection, new HadithReference("USC", ["1"], null));
        var second = Build(collection, new HadithReference("USC", ["2"], null));

        int result = comparer.Compare(first, second);

        Assert.True(result < 0);
    }

    [Fact]
    public void Compare_SameReferenceTypeReversed_ReturnsPositive()
    {
        var collection = Collection();
        var comparer = new HadithComparer(collection.ReferenceDefinitions);
        var first = Build(collection, new HadithReference("USC", ["2"], null));
        var second = Build(collection, new HadithReference("USC", ["1"], null));

        int result = comparer.Compare(first, second);

        Assert.True(result > 0);
    }

    [Fact]
    public void Compare_EqualReferences_ReturnsZero()
    {
        var collection = Collection();
        var comparer = new HadithComparer(collection.ReferenceDefinitions);
        var first = Build(collection, new HadithReference("USC", ["1"], null));
        var second = Build(collection, new HadithReference("USC", ["1"], null));

        int result = comparer.Compare(first, second);

        Assert.Equal(0, result);
    }
}
