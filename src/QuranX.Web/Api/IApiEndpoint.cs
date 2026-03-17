using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace QuranX.Web.Api;

internal interface IApiEndpoint
{
    static abstract void Register(RouteGroupBuilder builder);
}
