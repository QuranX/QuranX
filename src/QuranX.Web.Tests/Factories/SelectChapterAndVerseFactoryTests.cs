using NSubstitute;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Factories;
using QuranX.Web.Models;

namespace QuranX.Web.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class SelectChapterAndVerseFactoryTests
{
    private static IChapterRepository BuildChapterRepo()
    {
        var repo = Substitute.For<IChapterRepository>();
        repo.Get(Arg.Any<int>()).Returns(call => QuranStructure.Chapter(call.Arg<int>()));
        return repo;
    }

    private static IVerseRepository BuildVerseRepo()
    {
        var repo = Substitute.For<IVerseRepository>();
        repo.GetVerseReferences().Returns(
        [
            new VerseReference(1, 1),
            new VerseReference(1, 2),
            new VerseReference(2, 1),
        ]);
        return repo;
    }

    [Fact]
    public void CreateForAllChaptersAndVerses_ReturnsModelWithSelection()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        var factory = new SelectChapterAndVerseFactory(
            BuildChapterRepo(),
            BuildVerseRepo(),
            commentaryRepo);

        SelectChapterAndVerse result = factory.CreateForAllChaptersAndVerses(
            selectedChapterNumber: 2,
            selectedVerseNumber: 1,
            url: "/Quran");

        Assert.Equal(2, result.SelectedChapterNumber);
        Assert.Equal(1, result.SelectedVerseNumber);
        Assert.EndsWith("/", result.Url);
    }

    [Fact]
    public void CreateForAllChaptersAndVerses_CachesAcrossCalls()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        var verseRepo = BuildVerseRepo();
        var factory = new SelectChapterAndVerseFactory(
            BuildChapterRepo(),
            verseRepo,
            commentaryRepo);

        factory.CreateForAllChaptersAndVerses(1, 1, "/A");
        factory.CreateForAllChaptersAndVerses(2, 1, "/B");

        verseRepo.Received(1).GetVerseReferences();
    }

    [Fact]
    public void CreateForCommentary_FiltersByCommentatorRanges()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo.GetVerseRangeReferences("Kathir").Returns(
        [
            new VerseRangeReference(1, 1, 1),
            new VerseRangeReference(2, 1, 1),
        ]);
        var factory = new SelectChapterAndVerseFactory(
            BuildChapterRepo(),
            BuildVerseRepo(),
            commentaryRepo);

        SelectChapterAndVerse result = factory.CreateForCommentary(
            commentatorCode: "Kathir",
            selectedChapterNumber: 2,
            selectedVerseNumber: 1,
            url: "/Tafsir/Kathir/");

        Assert.Equal(2, result.AvailableChapters.Count());
    }

    [Fact]
    public void CreateForCommentary_CachesByCommentatorCode()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo.GetVerseRangeReferences("Kathir").Returns(
        [
            new VerseRangeReference(1, 1, 1),
        ]);
        var factory = new SelectChapterAndVerseFactory(
            BuildChapterRepo(),
            BuildVerseRepo(),
            commentaryRepo);

        factory.CreateForCommentary("Kathir", 1, 1, "/A");
        factory.CreateForCommentary("Kathir", 1, 1, "/A");

        commentaryRepo.Received(1).GetVerseRangeReferences("Kathir");
    }
}
