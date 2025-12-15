using System.Diagnostics;

namespace Riff.Api.Middleware;

public class PerformanceWarningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceWarningMiddleware> _logger;
    
    private const int WarningThresholdMs = 500; 

    public PerformanceWarningMiddleware(RequestDelegate next, ILogger<PerformanceWarningMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > WarningThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs);
            }
        }
    }
}