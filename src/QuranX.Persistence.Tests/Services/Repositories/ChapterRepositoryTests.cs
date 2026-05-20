using QuranX.Persistence.Services.Repositories;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class ChapterRepositoryTests
{
    private readonly ChapterRepository _repository = new();

    [Fact]
    public void Get_ReturnsChapterFromQuranStructure()
    {
        var chapter = _repository.Get(1);
        Assert.Equal(1, chapter.ChapterNumber);
        Assert.Equal(7, chapter.NumberOfVerses);
    }

    [Fact]
    public void GetAll_ReturnsAll114Chapters()
    {
        Assert.Equal(114, _repository.GetAll().Count());
    }

    [Fact]
    public void Get_UnknownChapter_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => _repository.Get(999));
    }
}
