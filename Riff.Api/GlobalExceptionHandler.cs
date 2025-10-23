using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Exceptions;

namespace Riff.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ResourceNotFoundException ex => CreateNotFoundProblemDetails(httpContext, ex),

            _ => CreateInternalServerErrorProblemDetails(httpContext, exception)
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; 
    }

    private ProblemDetails CreateNotFoundProblemDetails(HttpContext httpContext, ResourceNotFoundException ex)
    {
        var details = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = _env.IsDevelopment() ? ex.ToString() : $"{ex.ResourceName} with id {ex.ResourceId} not found"
        };

        return details;
    }

    private ProblemDetails CreateInternalServerErrorProblemDetails(HttpContext httpContext, Exception ex)
    {
        return new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = _env.IsDevelopment() ? ex.ToString() : "Internal Server Error"
        };
    }
}