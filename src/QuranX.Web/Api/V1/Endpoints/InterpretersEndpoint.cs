using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Api.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public partial class InterpretersEndpoint : IApiEndpoint
{
    public const string UrlPath = "/interpreters";

    private readonly IVerseRepository VerseRepository;

    public InterpretersEndpoint(IVerseRepository verseRepository)
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
                    InterpretersEndpointResult result = Execute();
                    return Results.Ok(result);
                }
            )
            .WithName("GetInterpreters")
            .WithSummary("Get interpreters list")
            .WithDescription("Returns the list of available interpreters")
            .Produces<InterpretersEndpointResult>();
    }

    public InterpretersEndpointResult Execute()
    {
        Verse verse = VerseRepository.GetVerse(new Shared.Models.VerseReference(1, 1));
        IEnumerable<Interpreter> interpreters =
            verse
            .VerseTexts
            .OrderBy(x => x.TranslatorName)
            .ThenBy(x => x.TranslatorCode)
            .Select(x => new Interpreter(code: x.TranslatorCode, name: x.TranslatorName));
        return new InterpretersEndpointResult(interpreters);
    }

    public class InterpretersEndpointResult
    {
        public IEnumerable<Interpreter> Interpreters { get; }

        public InterpretersEndpointResult(IEnumerable<Interpreter> interpreters)
        {
            Interpreters = interpreters ?? throw new ArgumentNullException(nameof(interpreters));
        }
    }
}
