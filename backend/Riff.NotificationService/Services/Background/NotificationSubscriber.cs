using EasyNetQ;
using Riff.Api.Contracts.Messages;
using Riff.NotificationService.Extensions;
using Riff.NotificationService.Handlers;

namespace Riff.NotificationService.Services.Background;

public class NotificationSubscriber(
    IBus bus,
    IServiceProvider serviceProvider,
    ILogger<NotificationSubscriber> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subId = "riff.notifications";

        logger.LogInformation("Initializing RabbitMQ subscriptions...");

        try
        {
            await bus.SubscribeWithTracingAsync<TrackAddedEvent, TrackAddedHandler>(serviceProvider, subId);
            await bus.SubscribeWithTracingAsync<PlaybackStateChangedEvent, PlaybackStateHandler>(serviceProvider,
                subId);
            await bus.SubscribeWithTracingAsync<VoteUpdatedEvent, VoteUpdatedHandler>(serviceProvider, subId);
            await bus.SubscribeWithTracingAsync<TrackRemovedEvent, TrackRemovedHandler>(serviceProvider, subId);

            logger.LogInformation("Subscriptions active!");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to subscribe to events.");
            throw;
        }
    }
}