using EasyNetQ;
using EasyNetQ.Interception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riff.Infrastructure.Configuration;
using Riff.Infrastructure.Messaging;
using IEventBus = Riff.Infrastructure.Messaging.IEventBus;

namespace Riff.Infrastructure.Extensions;

public static class EasyNetQSetup
{
    public static IServiceCollection AddRiffEasyNetQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = new RabbitMqOptions();
        configuration.GetSection(RabbitMqOptions.SectionName).Bind(rabbitOptions);

        var connString = $"host={rabbitOptions.Host};username={rabbitOptions.Username};password={rabbitOptions.Password};publisherConfirms=true";

        services.AddEasyNetQ(connString)
            .UseSystemTextJson(); 
        
        services.AddSingleton<IProduceConsumeInterceptor, OpenTelemetryInterceptor>();
        services.AddSingleton<IEventBus, RiffEventBus>();
        return services;
    }
}