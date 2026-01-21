using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Riff.Infrastructure.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddRiffObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddPrometheusExporter()
                .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://aspire-dashboard:18889"); }))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource("Riff.*")
                    .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://aspire-dashboard:18889"); });
            });

        return services;
    }
}