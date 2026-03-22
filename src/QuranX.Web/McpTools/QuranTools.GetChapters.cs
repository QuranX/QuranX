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
    [Description("Gets information about chapters of the Quran, including number of verses.")]
    public GetChaptersResult GetChapters()
    {
        IEnumerable<ChapterData> chapters = ChapterRepository.GetAll();

        return new GetChaptersResult
        {
            Chapters = chapters.ToArray()
        };
    }

    public sealed class GetChaptersResult
    {
        public required ChapterData[] Chapters { get; init; }
    }
}

