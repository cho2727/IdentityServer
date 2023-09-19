using IdentityServer.Shared.Injectables;

namespace IdentityServer.Shared.Interface;

public interface IEndPoints : ITransient
{
    void AddEndPoints(IEndpointRouteBuilder routes);
}
