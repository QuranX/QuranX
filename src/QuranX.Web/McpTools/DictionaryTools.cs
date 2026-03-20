#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public sealed partial class DictionaryTools
{
    private readonly IDictionaryRepository DictionaryRepository;
    private readonly IDictionaryEntryRepository DictionaryEntryRepository;

    public DictionaryTools(
        IDictionaryRepository dictionaryRepository,
        IDictionaryEntryRepository dictionaryEntryRepository)
    {
        DictionaryRepository = dictionaryRepository;
        DictionaryEntryRepository = dictionaryEntryRepository;
    }
}
