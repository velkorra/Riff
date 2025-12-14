using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riff.Infrastructure.Interceptors;


namespace Riff.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRiffDbContext(this IServiceCollection services)
    {
        services.AddSingleton<AuditInterceptor>();
        Action<IServiceProvider, DbContextOptionsBuilder> configureOptions = (sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditInterceptor>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("default"));
            options.AddInterceptors(interceptor);
        };

        // services.AddDbContextFactory<RiffContext>(configureOptions);
        services.AddDbContext<RiffContext>(configureOptions);
        services.AddDataProtection()
            .PersistKeysToDbContext<RiffContext>();
        return services;
    }
}