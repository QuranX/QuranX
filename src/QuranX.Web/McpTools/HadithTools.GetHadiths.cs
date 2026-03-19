using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    [McpServerTool(
        Name = "get_hadiths",
        Title = "Gets specific hadiths from one or more collections",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets specific hadiths from one or more collections")]
    public GetHadithsResult GetHadiths(
        [Description("One or more hadith references identifying which hadiths to return.")]
        HadithReference[] hadithReferences)
    {
        var hadiths = HadithRepository.GetHadiths(hadithReferences);
        return new GetHadithsResult {
            RequestedHadithReferences = hadithReferences,
            Hadiths = hadiths.ToArray()
        };
    }

    public sealed class GetHadithsResult
    {
        public required HadithReference[] RequestedHadithReferences { get; init; }
        public required Hadith[] Hadiths { get; init; }
    }
}
