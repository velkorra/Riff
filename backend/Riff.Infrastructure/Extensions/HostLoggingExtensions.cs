using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Riff.Infrastructure.Extensions;

public static class HostLoggingExtensions
{
    public static IHostBuilder UseRiffLogger(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var otlpEndpoint = "http://aspire-dashboard:18889";

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Env", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Service", context.HostingEnvironment.ApplicationName)
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = otlpEndpoint;
                    options.Protocol = OtlpProtocol.Grpc;

                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = context.HostingEnvironment.ApplicationName
                    };
                });
        });
    }
}