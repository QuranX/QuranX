using Microsoft.AspNetCore.Routing;

namespace QuranX.Web.Api;

internal interface IApiEndpoint
{
    void Register(RouteGroupBuilder builder);
}
