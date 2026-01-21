using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Riff.Api;
using Riff.Api.Contracts.Protos;
using Riff.Api.Extensions;
using Riff.Api.GraphQL.Mutations;
using Riff.Api.GraphQL.Queries;
using Riff.Api.GraphQL.Types;
using Riff.Api.Services;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure.Extensions;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using IRoomService = Riff.Api.Services.Interfaces.IRoomService;
using ITrackService = Riff.Api.Services.Interfaces.ITrackService;
using IUserService = Riff.Api.Services.Interfaces.IUserService;
using RoomService = Riff.Api.Services.RoomService;
using TrackService = Riff.Api.Services.TrackService;
using UserService = Riff.Api.Services.UserService;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseRiffLogger();

    builder.Services.AddControllers();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddScoped<IRoomService, RoomService>();
    builder.Services.AddScoped<ITrackService, TrackService>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddScoped<IResourceLinker, ResourceLinker>();

    var playlistUrl = builder.Configuration["PlaylistService:Url"] ?? "http://localhost:5001";
    builder.Services.AddGrpcClient<Playlist.PlaylistClient>(o => { o.Address = new Uri(playlistUrl); });
    builder.Services
        .AddGraphQLServer()
        .AddAuthorization()
        .AddQueryType<AppQuery>()
        .AddMutationType<AppMutation>()
        .AddType<RoomType>()
        .AddType<TrackType>()
        .AddType<UserType>();

    builder.Services.AddRiffDbContext();

    builder.Services.AddRiffObservability(builder.Configuration, "Riff.Api");
    var healthChecks = builder.Services.AddHealthChecks();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = "https://auth.local.oshideck.app";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,

                ValidIssuer = "https://auth.local.oshideck.app",

                ValidAudience = "riff_api",

                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddOpenApiWithSecurityRequirements();


    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication();
    builder.Services.AddEndpointsApiExplorer();

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
    
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapGraphQL();

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