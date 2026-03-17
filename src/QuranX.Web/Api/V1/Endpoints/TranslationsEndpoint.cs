using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class TranslationsEndpoint : IApiEndpoint
{
    public const string UrlPath = "/verses/translations";

    private readonly IVerseRepository VerseRepository;

    public TranslationsEndpoint(IVerseRepository verseRepository)
    {
        VerseRepository = verseRepository;
    }

    void IApiEndpoint.Register(RouteGroupBuilder builder)
    {
        builder
            .MapGet(
                UrlPath,
                () =>
                {
                    IEnumerable<TranslationsEndpointResult> result = Execute();
                    return Results.Ok(result);
                }
            )
            .WithName("GetVerseTranslations")
            .WithSummary("Get available verse translations")
            .WithDescription("Returns the list of available verse translations")
            .Produces<TranslationsEndpointResult[]>();
    }

    public IEnumerable<TranslationsEndpointResult> Execute()
    {
        Verse verse = VerseRepository.GetVerse(new Shared.Models.VerseReference(1, 1));
        return verse
            .VerseTexts
            .OrderBy(x => x.TranslatorCode)
            .Select(x => new TranslationsEndpointResult(code: x.TranslatorCode, name: x.TranslatorName));
    }


    public class TranslationsEndpointResult
    {
        public string Code { get; }
        public string Name { get; }

        public TranslationsEndpointResult(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
