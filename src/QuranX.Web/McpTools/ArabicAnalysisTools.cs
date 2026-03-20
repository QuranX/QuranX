#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public sealed partial class ArabicAnalysisTools
{
    private readonly IChapterRepository ChapterRepository;
    private readonly IVerseAnalysisRepository VerseAnalysisRepository;

    public ArabicAnalysisTools(
        IChapterRepository chapterRepository,
        IVerseAnalysisRepository verseAnalysisRepository)
    {
        ChapterRepository = chapterRepository;
        VerseAnalysisRepository = verseAnalysisRepository;
    }
}
