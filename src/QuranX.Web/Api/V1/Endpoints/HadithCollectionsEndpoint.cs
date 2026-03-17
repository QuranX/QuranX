using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Api.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class HadithCollectionsEndpoint : IApiEndpoint
{
    public const string UrlPath = "/hadith-collections";

    private IHadithCollectionRepository HadithCollectionRepository;

    public HadithCollectionsEndpoint(IHadithCollectionRepository hadithCollectionRepository)
    {
        HadithCollectionRepository = hadithCollectionRepository;
    }

    void IApiEndpoint.Register(RouteGroupBuilder builder)
    {
        builder
            .MapGet(
                UrlPath,
                () =>
                {
                    HadithCollectionsEndpointResult result = Execute();
                    return Results.Ok(result);
                }
            )
            .WithName("GetHadithCollections")
            .WithSummary("Get hadith collection list")
            .WithDescription("Returns the list of available hadith collections")
            .Produces<HadithCollectionsEndpointResult>();
    }

    public HadithCollectionsEndpointResult Execute()
    {
        IEnumerable<HadithCollection> hadithCollections =
            HadithCollectionRepository
            .GetAll()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Select(x =>
                new HadithCollection(
                    code: x.Code,
                    name: x.Name,
                    hadithCount: x.HadithCount,
                    primaryReferenceCode: x.PrimaryReferenceCode,
                    referenceDefinitions:
                        x
                        .ReferenceDefinitions
                        .Select(x =>
                            new HadithReferenceDefinition(
                                code: x.Code,
                                name: x.Name,
                                valuePrefix: x.ValuePrefix,
                                partNames: x.PartNames
                            )
                         )
                 )
            );
        return new HadithCollectionsEndpointResult(hadithCollections);

    }

    public class HadithCollectionsEndpointResult
    {
        public IEnumerable<HadithCollection> HadithCollections { get; }

        public HadithCollectionsEndpointResult(IEnumerable<HadithCollection> hadithCollections)
        {
            HadithCollections = hadithCollections ?? throw new ArgumentNullException(nameof(hadithCollections));
        }
    }

}
