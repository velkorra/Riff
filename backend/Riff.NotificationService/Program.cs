using System.Text.Json.Serialization;
using OpenTelemetry.Metrics;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Riff.Api.Contracts.Messages;
using Riff.NotificationService.Handlers;
using Riff.NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
    .AddJsonProtocol(options => { options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

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

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter());

var healthChecks = builder.Services.AddHealthChecks();

var rabbitConn = builder.Configuration["RabbitMq:ConnectionString"];
var queueName = builder.Configuration["RabbitMq:InputQueueName"];

builder.Services.AddRebus(configure => configure
    .Logging(l => l.Console())
    .Transport(t => t.UseRabbitMq(rabbitConn, queueName))
    .Routing(r => { r.TypeBased().MapAssemblyOf<TrackAddedEvent>("riff.playlist.service"); })
);

builder.Services.AutoRegisterHandlersFromAssemblyOf<TrackAddedHandler>();

var app = builder.Build();

app.UseCors();

app.MapHub<RiffHub>("/hub");

app.MapGet("/", () => "Riff Notification Service (SignalR + Rebus)");

var bus = app.Services.GetRequiredService<IBus>();

await bus.Subscribe<TrackAddedEvent>();
await bus.Subscribe<PlaybackStateChangedEvent>();
await bus.Subscribe<VoteUpdatedEvent>();
await bus.Subscribe<TrackRemovedEvent>();

app.MapPrometheusScrapingEndpoint();
app.MapHealthChecks("/health");

app.Run();