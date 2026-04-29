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
            Chapters = chapters.ToArray()
        };
    }

    public sealed class GetChaptersResult
    {
        public required ChapterData[] Chapters { get; init; }
    }
}

