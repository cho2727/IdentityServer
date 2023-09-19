using IdentityServer.Shared.Injectables;

namespace IdentityServer.Shared.Interfaces;

public interface ITokenGenerator : ISingleton
{
    string Create(string id, string role="user");

    string CreateRefreshToken();
}
