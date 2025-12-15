using Microsoft.AspNetCore.Identity;
using Riff.Api;
using Riff.Api.Contracts.Protos;
using Riff.Api.Extensions;
using Riff.Api.GraphQL.Mutations;
using Riff.Api.GraphQL.Queries;
using Riff.Api.GraphQL.Types;
using Riff.Api.Middleware;
using Riff.Api.Services;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;
using Riff.Infrastructure.Extensions;
using Scalar.AspNetCore;
using IRoomService = Riff.Api.Services.Interfaces.IRoomService;
using ITrackService = Riff.Api.Services.Interfaces.ITrackService;
using IUserService = Riff.Api.Services.Interfaces.IUserService;
using RoomService = Riff.Api.Services.RoomService;
using TrackService = Riff.Api.Services.TrackService;
using UserService = Riff.Api.Services.UserService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IResourceLinker, ResourceLinker>();

builder.Services.AddGrpcClient<Playlist.PlaylistClient>(o => { o.Address = new Uri("http://localhost:5000"); });

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<AppQuery>()
    .AddMutationType<AppMutation>()
    .AddType<RoomType>()
    .AddType<TrackType>()
    .AddType<UserType>();

builder.Services.AddRiffDbContext();


// builder.Services.AddDbContext<RiffContext>(options =>
//     options.UseInMemoryDatabase("RiffDb"));

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<RiffContext>();

builder.Services.AddOpenApiWithSecurityRequirements();


builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<RiffContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await DataSeeder.InitializeDatabase(context, userManager);
    }
}

app.UseMiddleware<CorrelationIdLoggingMiddleware>();
app.UseMiddleware<PerformanceWarningMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth").MapIdentityApi<User>();

app.MapControllers();
app.MapGraphQL();

app.Run();