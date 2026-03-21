#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class QuranTools
{
    public const string GetAvailableTranslatorsName = "get_available_translators";

    [McpServerTool(
        Name = GetAvailableTranslatorsName,
        Title = "Get Quran available translators",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets a list of available translators.")]
    public GetAvailableTranslatorsResult GetAvailableTranslators()
    {
        Verse firstVerse = VerseRepository.GetVerse(new VerseReference(1, 1));
        Translator[] translators =
            firstVerse
            .VerseTexts
            .Select(x =>
                new Translator
                {
                    Code = x.TranslatorCode,
                    Name = x.TranslatorName
                }
            )
            .ToArray();

        return new GetAvailableTranslatorsResult
        {
            Translators = translators
        };
    }

    public sealed class GetAvailableTranslatorsResult
    {
        public required Translator[] Translators { get; init; }
    }

    public sealed class Translator
    {
        public required string Code { get; init; }
        public required string Name { get; init; }
    }
}

