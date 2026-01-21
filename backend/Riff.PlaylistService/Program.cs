using Riff.Infrastructure.Extensions;
using Riff.PlaylistService.Services;
using Riff.PlaylistService.Services.Background;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseRiffLogger();

    builder.Services.AddGrpc();
    builder.Services.AddRiffDbContext();
    builder.Services.AddSingleton<PlayerOrchestrator>();
    builder.Services.AddHostedService<StartupStateRestorer>();
    builder.Services.AddRiffEasyNetQ(builder.Configuration);

    builder.Services.AddRiffObservability(builder.Configuration, "Riff.Playlist");

    var healthChecks = builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex != null || httpContext.Response.StatusCode >= 500)
                return LogEventLevel.Error;

            if (httpContext.Request.Path == "/health" ||
                httpContext.Request.Path == "/ready" ||
                httpContext.Request.Path == "/metrics")
                return LogEventLevel.Verbose;

            if (httpContext.Response.StatusCode >= 400)
            {
                return LogEventLevel.Warning;
            }
            
            return LogEventLevel.Information;
        };

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var user = httpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(user))
            {
                diagnosticContext.Set("UserName", user);
            }
        };
    });

    app.MapGrpcService<PlaylistGrpcService>();

    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

    app.MapPrometheusScrapingEndpoint();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}