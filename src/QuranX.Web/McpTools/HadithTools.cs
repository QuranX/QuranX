using ModelContextProtocol.Server;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public partial class HadithTools
{
    private readonly IHadithCollectionRepository HadithCollectionRepository;
    private readonly IHadithRepository HadithRepository;

    public HadithTools(IHadithCollectionRepository hadithCollectionRepository, IHadithRepository hadithRepository)
    {
        HadithCollectionRepository = hadithCollectionRepository;
        HadithRepository = hadithRepository;
    }
}
