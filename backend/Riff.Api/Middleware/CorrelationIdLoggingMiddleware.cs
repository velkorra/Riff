using System.Diagnostics;

namespace Riff.Api.Middleware;

public class CorrelationIdLoggingMiddleware
{
    private const string CorrelationIdHeader = "X-Request-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdLoggingMiddleware> _logger;

    public CorrelationIdLoggingMiddleware(RequestDelegate next, ILogger<CorrelationIdLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrSetCorrelationId(context);
        
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "CID: {CorrelationId} | Request started: {Method} {Path}", 
            correlationId, 
            context.Request.Method, 
            context.Request.Path);
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "CID: {CorrelationId} | Request finished: {Method} {Path} with status {StatusCode} in {Duration}ms",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    private static string GetOrSetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationIdValues) && correlationIdValues.FirstOrDefault() is { } existingId)
        {
            context.Response.Headers.Append(CorrelationIdHeader, existingId);
            return existingId;
        }

        var newId = Guid.NewGuid().ToString();
        context.Request.Headers.Append(CorrelationIdHeader, newId);
        context.Response.Headers.Append(CorrelationIdHeader, newId);
        return newId;
    }
}