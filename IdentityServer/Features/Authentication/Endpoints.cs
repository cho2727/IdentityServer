using IdentityServer.Shared.Interface;

namespace IdentityServer.Features.Authentication;

public class Endpoints : IEndPoints
{
    public void AddEndPoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("authentication")
            .WithOpenApi()
            .RequireAuthorization("CustomAuthorizationPolicy");
        group.MapGet("/", () => TypedResults.Ok("Hello"));
    }
}
