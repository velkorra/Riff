using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Riff.ApiGateway;
using Riff.ApiGateway.Services;
using Riff.ApiGateway.Services.Interfaces;
using Riff.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();



builder.Services.AddScoped<IRoomService, RoomService>();

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
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GrooveQueue API v1");
        options.RoutePrefix = string.Empty;
    });

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<RiffContext>();

        DataSeeder.InitializeDatabase(context);
    }
}

app.UseAuthorization();

app.MapControllers();

app.Run();