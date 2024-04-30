using System;
using System.Net;
using System.Threading.Tasks;
using FitnessApp.Common.Serializer.JsonSerializer;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace FitnessApp.Common.Middleware
{
    public abstract class AbstractErrorHandlerMiddleware(RequestDelegate next, IJsonSerializer serializer)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception error)
            {
                await HandleGlobalError(context, error);
                Log.Error(error, "");
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)GetStatusCodeByError(error);
                var result = serializer.SerializeToString(new { message = "Ach-ach-ach" });
                await response.WriteAsync(result);
            }
        }

        protected string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(MiddlewareConstants.CorrelationIdHeaderName, out var correlationId);
            return correlationId;
        }

        protected abstract Task HandleGlobalError(HttpContext context, Exception error);

        protected abstract HttpStatusCode GetStatusCodeByError(Exception error);
    }
}
