using IdentityServer.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace IdentityServer.Authorization
{
    public class CustomAuthorizationRequirement : IAuthorizationRequirement
    {

    }

    public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizationRequirement>
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthorizationHandler(ILogger<CustomAuthorizationHandler> logger, 
                                        IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizationRequirement requirement)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string jwtToken = string.Empty;
            var token = request?.Headers["Authorization"];
            AuthenticationHeaderValue.TryParse(token, out var tokenValue);
            jwtToken = tokenValue?.Parameter ?? "";

            jwtToken = jwtToken == "" ? request?.Query["access_token"].FirstOrDefault() ?? string.Empty : jwtToken;

            if (ValidateJweToken(jwtToken))
            {
                _logger.LogDebug("authorization succeed");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogDebug("authorization failed");
                context.Fail();
            }

            return Task.CompletedTask;
        }

        private bool ValidateJweToken(string token)
        {
            try
            {
                var handler = new JsonWebTokenHandler();
                TokenValidationResult result = handler.ValidateToken
                                    (token,
                                    new TokenValidationParameters
                                    {
                                        ValidAudience = "IdentityServer",
                                        ValidIssuer = "https://localhost:7172",

                                        // public key for signing
                                        IssuerSigningKey = JwtTokenConfigHelper.PublicSigningKey,

                                        // private key for encryption
                                        TokenDecryptionKey = JwtTokenConfigHelper.PrivateEncryptionKey
                                    });

                return result.IsValid;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }

            return false;
        }

    }
}
