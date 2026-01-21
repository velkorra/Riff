using System.Diagnostics;
using EasyNetQ;

namespace Riff.Infrastructure.Messaging;

public class RiffEventBus(IBus bus) : IEventBus
{
    private static readonly ActivitySource ActivitySource = new("Riff.Infrastructure");

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity($"Publish {typeof(T).Name}", ActivityKind.Producer);

        try
        {
            await bus.PubSub.PublishAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}