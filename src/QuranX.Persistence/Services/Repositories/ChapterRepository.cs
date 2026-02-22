using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Persistence.Services.Repositories;

public interface IChapterRepository
{
    ChapterData Get(int chapterNumber);
    IEnumerable<ChapterData> GetAll();
}

public class ChapterRepository : IChapterRepository
{
    public ChapterData Get(int chapterNumber) => QuranStructure.Chapter(chapterNumber);
    public IEnumerable<ChapterData> GetAll() => QuranStructure.Chapters;
}