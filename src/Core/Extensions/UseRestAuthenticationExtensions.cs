using Core.Authentication;
using Microsoft.AspNetCore.Builder;

namespace Core.Extensions
{
    public static class UseRestAuthenticationExtensions
    {
        public static IApplicationBuilder UseRestAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RestMiddleware>();
        }
    }
}