#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public sealed partial class QuranTools
{
    private readonly IChapterRepository ChapterRepository;
    private readonly IVerseRepository VerseRepository;

    public QuranTools(IChapterRepository chapterRepository, IVerseRepository verseRepository)
    {
        ChapterRepository = chapterRepository;
        VerseRepository = verseRepository;
    }
}