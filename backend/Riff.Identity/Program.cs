using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Riff.Identity;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;
using Riff.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseRiffLogger();

    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<RiffContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("default")));

    builder.Services.AddIdentity<User, IdentityRole<Guid>>()
        .AddEntityFrameworkStores<RiffContext>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    builder.Services.AddIdentityServer(options =>
        {
            options.IssuerUri = "https://auth.local.oshideck.app";

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            options.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryClients(Config.Clients)
        .AddInMemoryApiResources(Config.ApiResources)
        .AddAspNetIdentity<User>()
        .AddDeveloperSigningCredential();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

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

    app.UseStaticFiles();
    app.UseRouting();

    app.UseForwardedHeaders();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapDefaultControllerRoute();

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