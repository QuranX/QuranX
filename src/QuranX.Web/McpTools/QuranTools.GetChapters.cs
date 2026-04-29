#nullable enable
using ModelContextProtocol.Server;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class QuranTools
{
    public const string GetChaptersName = "get_chapters";

    [McpServerTool(
        Name = GetChaptersName,
        Title = "Get Quran chapters",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Lists all 114 Quran chapters with numbers, names (Arabic + English), revelation
        order, place of revelation (Mecca/Medina), and verse counts. Use to map a chapter
        name supplied by the user (e.g. 'Surah Al-Baqarah') to a chapter number, or to
        size a verse range correctly before calling {{GetVersesName}}. Verse content
        itself comes from {{GetVersesName}}.
        """)]
    public GetChaptersResult GetChapters()
    {
        IEnumerable<ChapterData> chapters = ChapterRepository.GetAll();

        return new GetChaptersResult
        {
            Chapters = chapters.ToArray(),
            NextSteps =
                $$"""
                Use the chapter numbers to construct verse references for
                {{GetVersesName}}, {{CommentaryTools.GetCommentariesForVerseName}},
                {{HadithTools.GetHadithsForVerseName}}, or
                {{ArabicAnalysisTools.GetVerseRootWordAnalysisName}}. To find verses
                by topic across all chapters, use {{SearchTools.SearchName}} with
                context={{nameof(SearchTools.SearchContext.Quran)}}.
                """
        };
    }

    public sealed class GetChaptersResult
    {
        public required ChapterData[] Chapters { get; init; }

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }
    }
}

