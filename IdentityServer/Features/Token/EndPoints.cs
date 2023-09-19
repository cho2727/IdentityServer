using IdentityServer.Shared.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Features.Token;

public class EndPoints : IEndPoints
{
    public void AddEndPoints(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("token").WithOpenApi();
        group.MapPost("generate/jwt", async([FromBody]Generate.Command request
            , IMediator mediator) => await mediator.Send(request));
    }
}
