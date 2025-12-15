using Riff.Api.Contracts.Enums;

namespace Riff.Api.Contracts.Messages;

public record TrackAddedEvent(
    Guid RoomId, 
    Guid TrackId, 
    string Title, 
    string Artist, 
    int DurationInSeconds,
    string AddedByUserId
);
public record PlaybackStateChangedEvent(
    Guid RoomId,
    Guid? CurrentTrackId,
    TrackStatus Status,
    double PositionInSeconds
);

public record VoteUpdatedEvent(
    Guid RoomId,
    Guid TrackId,
    int NewScore
);

public record TrackRemovedEvent(
    Guid RoomId,
    Guid TrackId
);