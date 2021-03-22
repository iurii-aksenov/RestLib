using System;
using Core.ExceptionsHandling;
using Microsoft.AspNetCore.Builder;

namespace Core.Extensions
{
    public static class UseRestExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseRestExceptionHandling(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException($"{nameof(UseRestExceptionHandling)}: {nameof(app)} is null.");
            }

            return app.UseMiddleware<RestExceptionHandlingMiddleware>();
        }
    }
}