using Serilog.Context;

namespace Bookify.Api.Middleware;

public class RequestContextLoggingMiddleware
{
    private const string _correlationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(httpContext)))
        {
            await _next(httpContext);
        }
    }

    private static string GetCorrelationId(HttpContext httpContext)
    {
        httpContext.Request.Headers.TryGetValue(_correlationIdHeaderName, out var correlationId);

        return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;        
    }


}
// this implementation gives us flexibility of either taking in the correlationId externally from another service taking with the api. allowing us to 
// trace a single request across multiple services in a microservice enviroment. otherwise we use traceIdentifier.