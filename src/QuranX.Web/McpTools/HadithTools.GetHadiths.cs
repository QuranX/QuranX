using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    public const string GetHadithsName = "get_hadiths";

    [McpServerTool(
        Name = GetHadithsName,
        Title = "Gets specific hadiths from one or more collections",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets specific hadiths from one or more collections.")]
    public GetHadithsResult GetHadiths(
        [Description("One or more hadith references identifying which hadiths to return.")]
        HadithReference[] hadithReferences)
    {
        if (Activity.Current is Activity activity)
            activity.SetTag("mcp.tool.arg.HadithReferences", JsonSerializer.Serialize(hadithReferences));

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
