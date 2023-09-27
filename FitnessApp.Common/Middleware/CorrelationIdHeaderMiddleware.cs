using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FitnessApp.Common.Middleware
{
    public class CorrelationIdHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(MiddlewareConstants.CorrelationIdHeaderName, out var correlationId))
                correlationId = Guid.NewGuid().ToString();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(MiddlewareConstants.CorrelationIdHeaderName, new[] { correlationId.ToString() });
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
