using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Core.Authentication
{
    public class RestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RestOptions _restSettings;

        public RestMiddleware(RequestDelegate next, RestOptions restSettings)
        {
            _next = next;
            _restSettings = restSettings;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Rest-Auth", out var header))
            {
                var isValid = false;
                var claims = new JwtSecurityTokenHandler().ValidateToken(header.Last(),
                    CreateTokenValidationParams(_restSettings), out var token);
                if (claims != null && token != null)
                {
                    isValid = true;
                }

                context.Features.Set(new RestAuthenticationFeature { IsValid = isValid });
            }

            await _next.Invoke(context);
        }

        private static TokenValidationParameters CreateTokenValidationParams(RestOptions restSettings)
        {
            return new()
            {
                ValidateAudience = true,
                ValidAudience = restSettings.RestToken.Audience,
                ValidateIssuer = true,
                ValidIssuer = restSettings.RestToken.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(restSettings.RestToken.IssuerSigningKey)),
                ValidateLifetime = true
            };
        }
    }
}