using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    public const string GetAvailableHadithCollectionsName = "get_available_hadith_collections";

    [McpServerTool(
        Name = GetAvailableHadithCollectionsName,
        Title = "Get a list of available hadith collections and hadith reference definitions",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Get a list of available hadith collections and hadith reference definitions.")]
    public GetHadithCollectionsResult GetHadithCollections()
    {
        IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();
        return new GetHadithCollectionsResult { HadithCollections = collections.ToArray() };
    }

    public sealed class GetHadithCollectionsResult
    {
        public required HadithCollection[] HadithCollections { get; init; }
    }
}
