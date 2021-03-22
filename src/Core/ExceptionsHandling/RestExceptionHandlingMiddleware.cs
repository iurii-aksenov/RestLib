using System;
using System.Net;
using System.Threading.Tasks;
using Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.ExceptionsHandling
{
    public class RestExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public RestExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHostEnvironment env,
            ILogger<RestExceptionHandlingMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (context.Request.Headers.ContainsKey("Rest-Auth"))
            {
                logger.LogError(ex, ex.Message, ex.InnerException?.Message);
                await HandleExceptionAsync(context, env, ex);
            }
        }

        protected virtual RestError HandleCustomException(RestError defaultError, HttpContext context,
            IHostEnvironment env, Exception exception)
        {
            // Sample
            // switch (exception)
            // {
            //     case CustomErrorType.NotFoundException ex:
            //         errorType = CustomErrorType.NotFoundException;
            //         errorCode = ex.Code;
            //         break;
            //     case CustomErrorType.ForbiddenException ex:
            //         errorType = CustomErrorType.ForbiddenException;
            //         errorCode = ex.Code;
            //         break;
            //     case CustomErrorType.Exception ex:
            //         errorType = CustomErrorType.Exception;
            //         errorCode = ex.Code;
            //         break;
            // }


            return defaultError;
        }

        private async Task HandleExceptionAsync(HttpContext context, IHostEnvironment env, Exception exception)
        {
            const RestErrorType errorType = RestErrorType.SystemException;
            var errorCode = string.Empty;
            var message = exception.Message + exception.InnerException?.Message;
            var stackTrace = env.IsDevelopment() || env.IsStaging() ? exception.StackTrace : string.Empty;

            var error = new RestError((int) errorType, message, errorCode, stackTrace);

            error = HandleCustomException(error, context, env, exception);

            var response = new RestResponse<object>(error);

            var result = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            await context.Response.WriteAsync(result);
        }
    }
}