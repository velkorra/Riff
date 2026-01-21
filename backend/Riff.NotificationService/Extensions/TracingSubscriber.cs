using System.Diagnostics;
using System.Text;
using EasyNetQ;
using EasyNetQ.Consumer;
using Riff.NotificationService.Handlers;

namespace Riff.NotificationService.Extensions;

public static class TracingSubscriber
{
    private static readonly ActivitySource ActivitySource = new("Riff.NotificationService");

    public static async Task SubscribeWithTracingAsync<TEvent, THandler>(
        this IBus bus,
        IServiceProvider serviceProvider,
        string subscriptionId)
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        var typeNameSerializer = serviceProvider.GetRequiredService<ITypeNameSerializer>();
        var typeName = typeNameSerializer.Serialize(typeof(TEvent));

        var exchangeName = typeName;

        var queueName = $"{typeName}_{subscriptionId}";

        var exchange = await bus.Advanced.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Topic
        );

        var queue = await bus.Advanced.QueueDeclareAsync(
            queueName,
            _ => { }
        );

        await bus.Advanced.QueueBindAsync(queue.Name, exchange.Name, "#");

        await bus.Advanced.ConsumeAsync(configuration =>
        {
            configuration.ForQueue(
                queue,
                (IHandlerRegistration registration) =>
                {
                    registration.Add<TEvent>(async (message, info, ct) =>
                    {
                        string? parentId = null;
                        if (message.Properties.Headers != null &&
                            message.Properties.Headers.TryGetValue("traceparent", out var val) &&
                            val is byte[] bytes)
                        {
                            parentId = Encoding.UTF8.GetString(bytes);
                        }

                        using var activity = ActivitySource.StartActivity(
                            $"Process {typeof(TEvent).Name}",
                            ActivityKind.Consumer,
                            parentId);

                        using var scope = serviceProvider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

                        try
                        {
                            await handler.Handle(message.Body);
                        }
                        catch (Exception ex)
                        {
                            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                            throw;
                        }
                    });
                },
                perQueueConfig => { perQueueConfig.WithAutoAck(); }
            );
        });
    }
}