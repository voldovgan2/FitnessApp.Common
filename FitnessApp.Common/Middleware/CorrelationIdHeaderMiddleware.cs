using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FitnessApp.Common.Middleware;

[ExcludeFromCodeCoverage]
public class CorrelationIdHeaderMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(MiddlewareConstants.CorrelationIdHeaderName, out var correlationId))
            correlationId = Guid.NewGuid().ToString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(MiddlewareConstants.CorrelationIdHeaderName, new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });

        await next(context);
    }
}
