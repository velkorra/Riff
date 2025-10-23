using Application.Services;
using Application.Services.Interfaces;
using GraphiQl;
using GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Riff.Api;
using Riff.Api.GraphQL;
using Riff.Api.GraphQL.Types;
using Riff.Api.GraphQL.Types.Input;
using Riff.Api.Middleware;
using Riff.Api.Services;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IResourceLinker, ResourceLinker>();

builder.Services.AddSingleton<UserType>();
builder.Services.AddSingleton<RoomType>();
builder.Services.AddSingleton<TrackType>();
builder.Services.AddSingleton<RegisterUserInputType>();
builder.Services.AddSingleton<CreateRoomInputType>();
builder.Services.AddSingleton<AddTrackInputType>();
builder.Services.AddSingleton<AppQuery>();
builder.Services.AddSingleton<AppMutation>();
builder.Services.AddSingleton<AppSchema>();

builder.Services.AddGraphQL(b => b
    .AddSchema<AppSchema>()
    .ConfigureExecution((opt, next) =>
    {
        opt.EnableMetrics = true;
        return next(opt);
    })
    .AddSystemTextJson()
    .AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = builder.Environment.IsDevelopment()));

builder.Services.AddDbContext<RiffContext>(options =>
    options.UseInMemoryDatabase("RiffDb"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Riff API",
        Description = "API for collaborative real-time music service"
    });

    options.EnableAnnotations();
    
    options.OperationFilter<InheritAttributesFromInterfacesFilter>();
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Riff API v1");
        options.RoutePrefix = string.Empty;
    });

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<RiffContext>();

        DataSeeder.InitializeDatabase(context);
    }
}

app.UseGraphQL<AppSchema>("/graphql");
app.UseGraphiQl("/graphiql", "/graphql");
app.UseMiddleware<CorrelationIdLoggingMiddleware>();
app.UseMiddleware<PerformanceWarningMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();