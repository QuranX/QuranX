using NSubstitute;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Factories;
using QuranX.Web.Models;

namespace QuranX.Web.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class CommentariesForVerseFactoryTests
{
    private static IChapterRepository BuildChapterRepo()
    {
        var repo = Substitute.For<IChapterRepository>();
        repo.Get(1).Returns(QuranStructure.Chapter(1));
        return repo;
    }

    private static ICommentatorRepository BuildCommentatorRepo()
    {
        var repo = Substitute.For<ICommentatorRepository>();
        repo.GetAll().Returns(
        [
            new Commentator("Kathir", "Ibn Kathir"),
            new Commentator("Jalal", "Jalalayn"),
        ]);
        return repo;
    }

    private static Commentary BuildCommentary(string code) => new(
        commentatorCode: code,
        chapterNumber: 1,
        firstVerseNumber: 1,
        lastVerseNumber: 1,
        text: []);

    [Fact]
    public void Create_AllCommentators_AggregatesCommentaries()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo.GetForVerse(1, 1).Returns([BuildCommentary("Kathir"), BuildCommentary("Jalal")]);
        var factory = new CommentariesForVerseFactory(
            BuildChapterRepo(),
            BuildCommentatorRepo(),
            commentaryRepo);

        CommentariesForVerse result = factory.Create(chapterNumber: 1, verseNumber: 1);

        Assert.Equal(2, result.Commentaries.Count());
        Assert.Equal(1, result.VerseNumber);
    }

    [Fact]
    public void Create_SingleCommentator_ReturnsFilteredCommentary()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo
            .GetForVerse("Kathir", 1, 1)
            .Returns(BuildCommentary("Kathir"));
        var factory = new CommentariesForVerseFactory(
            BuildChapterRepo(),
            BuildCommentatorRepo(),
            commentaryRepo);

        CommentariesForVerse result = factory.Create("Kathir", 1, 1);

        Assert.Single(result.Commentaries);
        Assert.Equal("Kathir", result.Commentaries.First().Commentator.Code);
    }

    [Fact]
    public void Create_SingleCommentatorMissing_ReturnsNoCommentaries()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo.GetForVerse("Jalal", 1, 1).Returns((Commentary)null!);
        var factory = new CommentariesForVerseFactory(
            BuildChapterRepo(),
            BuildCommentatorRepo(),
            commentaryRepo);

        CommentariesForVerse result = factory.Create("Jalal", 1, 1);

        Assert.Empty(result.Commentaries);
    }

    [Fact]
    public void Create_BuildsSelectChapterAndVerseModel()
    {
        var commentaryRepo = Substitute.For<ICommentaryRepository>();
        commentaryRepo.GetForVerse(1, 2).Returns([]);
        var factory = new CommentariesForVerseFactory(
            BuildChapterRepo(),
            BuildCommentatorRepo(),
            commentaryRepo);

        CommentariesForVerse result = factory.Create(1, 2);

        Assert.NotNull(result.SelectChapterAndVerse);
        Assert.Equal(1, result.SelectChapterAndVerse.SelectedChapterNumber);
        Assert.Equal(2, result.SelectChapterAndVerse.SelectedVerseNumber);
    }
}
