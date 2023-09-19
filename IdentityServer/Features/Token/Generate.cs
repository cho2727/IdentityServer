using IdentityServer.Shared.Interface;
using IdentityServer.Shared.Utility;
using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Features.Token;

public class Generate
{
    public class Command : IRequest<Response>
    {
        public string Id { get; set; }
    }

    public class Response : BaseResponse
    {
        public string? AccessToken { get; set; }
    }

    public class CommandHandler : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var response = new Response { Result = false };
            var tokenString = GenerateJweToken(request.Id);
            response.AccessToken = tokenString;
            return await Task.FromResult(response);
        }

        private string GenerateJweToken(string id)
        {
            var privateSigningKey = JwtTokenConfigHelper.PrivateSigningKey;
            var publicEncryptionKey = JwtTokenConfigHelper.PublicEncryptionKey;

            // JwtSecurityTokenHandler 의 향상된 버전 JsonWebTokenHandler
            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = "IdentityServer",
                Issuer = "https://localhost:7158",
                Claims = new Dictionary<string, object> { { "sub", id } },

                // private key for signing
                SigningCredentials = new SigningCredentials(
                    privateSigningKey,
                    SecurityAlgorithms.EcdsaSha256),

                // public key for encryption
                EncryptingCredentials = new EncryptingCredentials(
                    publicEncryptionKey,
                    SecurityAlgorithms.RsaOAEP,
                    SecurityAlgorithms.Aes256CbcHmacSha512),
                Expires = DateTime.UtcNow.AddMinutes(10)
            });

            return token;
        }
    }
}
