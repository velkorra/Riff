using Microsoft.AspNetCore.SignalR;
using Rebus.Handlers;
using Riff.Api.Contracts.Messages;
using Riff.NotificationService.Hubs;

namespace Riff.NotificationService.Handlers;

public class TrackAddedHandler(IHubContext<RiffHub> hub, ILogger<TrackAddedHandler> logger)
    : IHandleMessages<TrackAddedEvent>
{
    public async Task Handle(TrackAddedEvent message)
    {
        logger.LogInformation("[Event] Track added in room {RoomId}: {Title}", message.RoomId, message.Title);

        await hub.Clients.Group(message.RoomId.ToString())
            .SendAsync("TrackAdded", message);
    }
}

public class PlaybackStateHandler(IHubContext<RiffHub> hub, ILogger<PlaybackStateHandler> logger)
    : IHandleMessages<PlaybackStateChangedEvent>
{
    public async Task Handle(PlaybackStateChangedEvent message)
    {
        logger.LogInformation("[Event] Playback in room {RoomId} changed to {Status}. Track: {TrackId}",
            message.RoomId, message.Status, message.CurrentTrackId);

        await hub.Clients.Group(message.RoomId.ToString())
            .SendAsync("PlaybackChanged", message);
    }
}

public class VoteUpdatedHandler(IHubContext<RiffHub> hub, ILogger<VoteUpdatedHandler> logger)
    : IHandleMessages<VoteUpdatedEvent>
{
    public async Task Handle(VoteUpdatedEvent message)
    {
        logger.LogInformation("[Event] Vote updated in room {RoomId} for track {TrackId}. New Score: {Score}",
            message.RoomId, message.TrackId, message.NewScore);

        await hub.Clients.Group(message.RoomId.ToString())
            .SendAsync("ScoreUpdated", message);
    }
}

public class TrackRemovedHandler(IHubContext<RiffHub> hub, ILogger<TrackRemovedHandler> logger)
    : IHandleMessages<TrackRemovedEvent>
{
    public async Task Handle(TrackRemovedEvent message)
    {
        logger.LogInformation("[Event] Track removed in room {RoomId}: {TrackId}", message.RoomId, message.TrackId);

        await hub.Clients.Group(message.RoomId.ToString())
            .SendAsync("TrackRemoved", message);
    }
}