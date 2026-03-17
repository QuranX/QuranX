using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class InterpretationsEndpoint : IApiEndpoint
{
    public const string UrlPath = "/interpretations";

    private readonly IVerseRepository VerseRepository;

    public InterpretationsEndpoint(IVerseRepository verseRepository)
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
                    InterpretationsEndpointResult result = Execute();
                    return Results.Ok(result);
                }
            )
            .WithName("Interpretations")
            .WithSummary("Get interpretations list")
            .WithDescription("Returns the list of available interpretations")
            .Produces<InterpretationsEndpointResult>();
    }

    public InterpretationsEndpointResult Execute()
    {
        Verse verse = VerseRepository.GetVerse(new Shared.Models.VerseReference(1, 1));
        IEnumerable<Interpretation> interpretations =
            verse
            .VerseTexts
            .OrderBy(x => x.TranslatorCode)
            .Select(x => new Interpretation(code: x.TranslatorCode, name: x.TranslatorName));
        return new InterpretationsEndpointResult(interpretations);
    }

    public class InterpretationsEndpointResult
    {
        public IEnumerable<Interpretation> Interpretations { get; }

        public InterpretationsEndpointResult(IEnumerable<Interpretation> interpretations)
        {
            Interpretations = interpretations ?? throw new ArgumentNullException(nameof(interpretations));
        }
    }

    public class Interpretation
    {
        public string Code { get; }
        public string Name { get; }

        public Interpretation(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
