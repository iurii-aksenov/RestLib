using System;
using System.Net;
using System.Threading.Tasks;
using Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Core.Authentication
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class RestAuthAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rest = context.HttpContext.Features.Get<RestAuthenticationFeature>();
            if (!rest.IsValid)
            {
                var error = new RestError((int) RestErrorType.RestUnauthorized, nameof(RestErrorType.RestUnauthorized),
                    "Unauthorized");

                var response = new RestResponse<object>(error);
                var result = JsonConvert.SerializeObject(response);
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.HttpContext.Response.WriteAsync(result);
                return;
            }

            await next();
        }
    }
}