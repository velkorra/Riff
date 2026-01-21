using System.Text.Json.Serialization;
using EasyNetQ;
using Riff.Api.Contracts.Messages;
using Riff.Infrastructure.Extensions;
using Riff.NotificationService.Extensions;
using Riff.NotificationService.Handlers;
using Riff.NotificationService.Hubs;
using Riff.NotificationService.Services.Background;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseRiffLogger();

    builder.Services.AddSignalR()
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddRiffObservability(builder.Configuration, "Riff.Notification");

    builder.Services.AddRiffEasyNetQ(builder.Configuration);
    builder.Services.AddHostedService<NotificationSubscriber>();

    builder.Services.AddScoped<TrackAddedHandler>();
    builder.Services.AddScoped<VoteUpdatedHandler>();
    builder.Services.AddScoped<PlaybackStateHandler>();
    builder.Services.AddScoped<TrackRemovedHandler>();

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

    app.UseCors();

    app.MapHub<RiffHub>("/hub");

    app.MapGet("/", () => "Riff Notification Service (SignalR + EasyNetQ)");


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