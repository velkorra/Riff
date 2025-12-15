using Rebus.Config;
using Riff.Infrastructure.Configuration;

namespace Riff.PlaylistService.Extensions;

public static class RebusExtensions
{
    public static IServiceCollection AddRiffMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = new RabbitMqOptions();
        configuration.GetSection(RabbitMqOptions.SectionName).Bind(rabbitOptions);
        services.AddSingleton(rabbitOptions);

        services.AddRebus(configure => configure
            .Logging(l => l.Console())
            .Transport(t => t.UseRabbitMq(
                rabbitOptions.GetConnectionString(),
                rabbitOptions.QueueName))
        );

        return services;
    }
}