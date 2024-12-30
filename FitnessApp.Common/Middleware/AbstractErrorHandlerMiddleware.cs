using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace FitnessApp.Common.Middleware;

[ExcludeFromCodeCoverage]
public abstract class AbstractErrorHandlerMiddleware(RequestDelegate next)
{
    public const string CorrelationIdHeaderName = "X-Correlation-Id";
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
            var result = JsonSerializer.Serialize(new { message = "Ach-ach-ach" });
            await response.WriteAsync(result);
        }
    }

    protected static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId);
        return correlationId;
    }

    protected abstract Task HandleGlobalError(HttpContext context, Exception error);

    protected abstract HttpStatusCode GetStatusCodeByError(Exception error);
}
