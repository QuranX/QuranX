#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Services;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public partial class SearchTools
{
    private readonly ISearchEngine SearchEngine;

    public SearchTools(ISearchEngine searchEngine)
    {
        SearchEngine = searchEngine;
    }
}
