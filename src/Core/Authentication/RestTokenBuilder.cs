using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Extensions;
using Core.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace Core.Authentication
{
    public static class RestTokenBuilder
    {
        private const int ExpiredDurationInSeconds = 60;
        public static string CreateToken(RestToken restToken, int? expiringDurationInSeconds = null)
        {
            var now = DateTime.UtcNow;
            var jwtClaims = new List<Claim> { new(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString()) };
            var issuerKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(restToken.IssuerSigningKey));

            var signingCredentials = new SigningCredentials(issuerKey, SecurityAlgorithms.HmacSha256);

            var expires = now.AddSeconds(expiringDurationInSeconds ?? ExpiredDurationInSeconds);
            var jwt = new JwtSecurityToken(restToken.Issuer, restToken.Audience, jwtClaims, now, expires,
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}