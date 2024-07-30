using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Middleware;

[ExcludeFromCodeCoverage]
public static class MiddlewareConstants
{
    public const string CorrelationIdHeaderName = "X-Correlation-Id";
}
