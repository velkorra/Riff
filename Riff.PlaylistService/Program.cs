using Microsoft.AspNetCore.Builder;
using Riff.Infrastructure.Extensions;
using Riff.PlaylistService.Extensions;
using Riff.PlaylistService.Services;
using Riff.PlaylistService.Services.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddRiffDbContext();
builder.Services.AddSingleton<PlayerOrchestrator>();
builder.Services.AddHostedService<StartupStateRestorer>();
builder.Services.AddRiffMessaging(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<PlaylistGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();