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
    [Description(
        $$"""
        Lists the codes and names of available English translators of the Quran. Call
        before passing values to translatorCodes on {{GetVersesName}} to filter which
        translations are returned.
        """)]
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
            Translators = translators,
            NextSteps =
                $$"""
                Pass any of these {{nameof(Translator.Code)}} values (or an empty array
                for all translations) to {{GetVersesName}} via the translatorCodes
                parameter to filter which translations are returned.
                """
        };
    }

    public sealed class GetAvailableTranslatorsResult
    {
        public required Translator[] Translators { get; init; }

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }
    }

    public sealed class Translator
    {
        public required string Code { get; init; }
        public required string Name { get; init; }
    }
}

