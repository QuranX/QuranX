using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class HadithRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private const string CollectionCode = "Bukhari";

    public HadithRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
    }

    private static HadithCollection BukhariCollection() => new(
        code: CollectionCode,
        name: "Sahih al-Bukhari",
        referenceDefinitions:
        [
            new HadithReferenceDefinition(
                collectionCode: CollectionCode,
                code: "USC",
                name: "USC-MSA",
                valuePrefix: "",
                partNames: ["Volume", "Book", "Number"],
                isPrimary: true),
            new HadithReferenceDefinition(
                collectionCode: CollectionCode,
                code: "InBook",
                name: "In Book",
                valuePrefix: "",
                partNames: ["Book", "Number"],
                isPrimary: false),
        ],
        hadithCount: 1);

    private static Hadith MakeHadith(
        string primaryReferencePath,
        IEnumerable<HadithReference>? references = null,
        IEnumerable<VerseRangeReference>? verseRanges = null)
    {
        return new Hadith(
            collectionCode: CollectionCode,
            arabicText: ["arabic"],
            englishText: ["english text body"],
            verseRangeReferences: verseRanges ?? [],
            references: references ?? [],
            primaryReferenceCode: "USC",
            primaryReferencePath: primaryReferencePath);
    }

    private (IHadithRepository repo, IHadithCollectionRepository collectionRepo) BuildRepos()
    {
        var collectionRepo = new HadithCollectionRepository(_fixture.SearcherProvider);
        var repo = new HadithRepository(collectionRepo, _fixture.SearcherProvider);
        return (repo, collectionRepo);
    }

    private void SeedSingleHadith(
        out HadithReference primaryRef,
        out VerseRangeReference verseRange)
    {
        primaryRef = new HadithReference(
            collectionCode: CollectionCode,
            referenceCode: "USC",
            referenceValue1: 1,
            referenceValue2: 2,
            referenceValue3: 3,
            suffix: "",
            primaryReferencePath: "Bukhari/USC/Volume-1/Book-2/Number-3");

        verseRange = new VerseRangeReference(chapter: 2, firstVerse: 5, lastVerse: 7);
        VerseRangeReference capturedRange = verseRange;
        HadithReference capturedRef = primaryRef;

        _fixture.Reseed(writer =>
        {
            var collectionRepo = new HadithCollectionRepository(_fixture.SearcherProvider);
            var collectionWriteRepo = new HadithCollectionWriteRepository(
                new SeededWriterProvider(writer));
            collectionWriteRepo.Write(BukhariCollection());

            var hadithWriteRepo = new HadithWriteRepository(new SeededWriterProvider(writer));
            hadithWriteRepo.Write(MakeHadith(
                primaryReferencePath: capturedRef.PrimaryReferencePath,
                references: [capturedRef],
                verseRanges: [capturedRange]));
        });
    }

    private sealed class SeededWriterProvider : QuranX.Persistence.Services.ILuceneIndexWriterProvider
    {
        private readonly Lucene.Net.Index.IndexWriter _writer;
        public SeededWriterProvider(Lucene.Net.Index.IndexWriter writer) => _writer = writer;
        public Lucene.Net.Index.IndexWriter GetIndexWriter() => _writer;
    }

    [Fact]
    public void GetAllReferences_ReturnsReferencesForCollection()
    {
        SeedSingleHadith(out HadithReference primaryRef, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var references = repo.GetAllReferences(CollectionCode).ToList();

        Assert.Single(references);
        Assert.Equal(primaryRef.ReferenceValue1, references[0].ReferenceValue1);
    }

    [Fact]
    public void GetReferences_ByOneValue_ReturnsMatchingReference()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var references = repo
            .GetReferences(CollectionCode, "USC", values: [1], suffix: "")
            .ToList();

        Assert.Single(references);
    }

    [Fact]
    public void GetReferences_ByThreeValues_ReturnsMatchingReference()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var references = repo
            .GetReferences(CollectionCode, "USC", values: [1, 2, 3], suffix: "")
            .ToList();

        Assert.Single(references);
    }

    [Fact]
    public void GetReferences_NoMatch_ReturnsEmpty()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var references = repo
            .GetReferences(CollectionCode, "USC", values: [99], suffix: "")
            .ToList();

        Assert.Empty(references);
    }

    [Fact]
    public void HasReferences_WhenPresent_ReturnsTrue()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        Assert.True(repo.HasReferences(CollectionCode, "USC", values: [1], suffix: ""));
    }

    [Fact]
    public void HasReferences_WhenAbsent_ReturnsFalse()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        Assert.False(repo.HasReferences(CollectionCode, "USC", values: [42], suffix: ""));
    }

    [Fact]
    public void GetHadiths_ByPaths_ReturnsMatchingHadiths()
    {
        SeedSingleHadith(out HadithReference primaryRef, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo
            .GetHadiths(CollectionCode, primaryReferencePaths: [primaryRef.PrimaryReferencePath])
            .ToList();

        Assert.Single(hadiths);
        Assert.Equal(primaryRef.PrimaryReferencePath, hadiths[0].PrimaryReferencePath);
    }

    [Fact]
    public void GetHadiths_MultipleHadithsShareReferencePath_ReturnsAll()
    {
        const string sharedPath = "Bukhari/USC/Volume-1/Book-2/Number-3";
        _fixture.Reseed(writer =>
        {
            var collectionWriteRepo = new HadithCollectionWriteRepository(
                new SeededWriterProvider(writer));
            collectionWriteRepo.Write(BukhariCollection());

            var hadithWriteRepo = new HadithWriteRepository(new SeededWriterProvider(writer));
            hadithWriteRepo.Write(MakeHadith(primaryReferencePath: sharedPath));
            hadithWriteRepo.Write(MakeHadith(primaryReferencePath: sharedPath));
        });
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo
            .GetHadiths(CollectionCode, primaryReferencePaths: [sharedPath])
            .ToList();

        // Both hadiths share the single requested path; capping numHits at paths.Length (1) would
        // silently drop one. All matching documents must be returned.
        Assert.Equal(2, hadiths.Count);
        Assert.All(hadiths, h => Assert.Equal(sharedPath, h.PrimaryReferencePath));
    }

    [Fact]
    public void GetHadiths_NullPaths_ReturnsEmpty()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo.GetHadiths(CollectionCode, primaryReferencePaths: null!);
        Assert.Empty(hadiths);
    }

    [Fact]
    public void GetHadiths_EmptyPaths_ReturnsEmpty()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo.GetHadiths(CollectionCode, primaryReferencePaths: []);
        Assert.Empty(hadiths);
    }

    [Fact]
    public void GetHadiths_ByReferences_ReturnsMatchingHadiths()
    {
        SeedSingleHadith(out HadithReference primaryRef, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo.GetHadiths(references: [primaryRef]).ToList();

        Assert.Single(hadiths);
    }

    [Fact]
    public void GetHadiths_NullReferences_ReturnsEmpty()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        Assert.Empty(repo.GetHadiths(references: null!));
    }

    [Fact]
    public void GetForVerse_VerseInRange_ReturnsHadith()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo.GetForVerse(new VerseReference(2, 6)).ToList();
        Assert.Single(hadiths);
    }

    [Fact]
    public void GetForVerse_VerseNotInRange_ReturnsEmpty()
    {
        SeedSingleHadith(out _, out _);
        (IHadithRepository repo, _) = BuildRepos();

        var hadiths = repo.GetForVerse(new VerseReference(99, 1)).ToList();
        Assert.Empty(hadiths);
    }
}
