using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Enums;
using Riff.Api.Contracts.Messages;
using Riff.Api.Contracts.Protos;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;
using Riff.Infrastructure.Messaging;
using Riff.PlaylistService.Services.Background;

namespace Riff.PlaylistService.Services;

public class PlaylistGrpcService(
    RiffContext context,
    PlayerOrchestrator orchestrator,
    ILogger<PlaylistGrpcService> logger,
    IEventBus bus) : Playlist.PlaylistBase
{
    public override async Task<AddTrackResponse> AddTrack(AddTrackRequest request, ServerCallContext context1)
    {
        try
        {
            var roomId = Guid.Parse(request.RoomId);
            var userId = Guid.Parse(request.UserId);

            if (!await context.Rooms.AnyAsync(r => r.Id == roomId))
            {
                return new AddTrackResponse { Success = false, ErrorMessage = "Room not found" };
            }

            var track = new Track
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                AddedById = userId,
                Title = request.Title,
                Artist = request.Artist,
                Url = request.Url,
                DurationInSeconds = request.DurationSeconds,
                Status = TrackStatus.Pending,
                Score = 0,
                CreatedAt = DateTimeOffset.UtcNow
            };

            context.Tracks.Add(track);
            await context.SaveChangesAsync();

            logger.LogInformation("Track {TrackId} added to room {RoomId}", track.Id, roomId);

            await bus.PublishAsync(new TrackAddedEvent(
                track.RoomId, 
                track.Id, 
                track.Title, 
                track.Artist, 
                track.DurationInSeconds, 
                track.AddedById.ToString()
            ));

            return new AddTrackResponse { Success = true, TrackId = track.Id.ToString() };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding track");
            return new AddTrackResponse { Success = false, ErrorMessage = ex.Message };
        }
    }

    public override async Task<VoteResponse> Vote(VoteRequest request, ServerCallContext context1)
    {
        try
        {
            var trackId = Guid.Parse(request.TrackId);
            var userId = Guid.Parse(request.UserId);

            if (request.Value != 1 && request.Value != -1)
                return new VoteResponse { Success = false, ErrorMessage = "Vote value must be 1 or -1" };

            var track = await context.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);
            if (track == null)
                return new VoteResponse { Success = false, ErrorMessage = "Track not found" };

            var existingVote = await context.Set<Vote>()
                .FirstOrDefaultAsync(v => v.TrackId == trackId && v.UserId == userId);

            if (existingVote != null)
            {
                if (existingVote.Value != request.Value)
                {
                    track.Score -= existingVote.Value;
                    existingVote.Value = request.Value;
                    track.Score += request.Value;
                }
            }
            else
            {
                var newVote = new Vote { TrackId = trackId, UserId = userId, Value = request.Value };
                context.Set<Vote>().Add(newVote);
                track.Score += request.Value;
            }

            await context.SaveChangesAsync();
            
            await bus.PublishAsync(new VoteUpdatedEvent(
                track.RoomId, 
                track.Id,
                track.Score
            ));

            return new VoteResponse { Success = true, NewScore = track.Score };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error voting");
            return new VoteResponse { Success = false, ErrorMessage = ex.Message };
        }
    }

    public override async Task<RemoveTrackResponse> RemoveTrack(
        RemoveTrackRequest request,
        ServerCallContext callContext)
    {
        try
        {
            var trackId = Guid.Parse(request.TrackId);
            var userId = Guid.Parse(request.UserId);

            var track = await context.Tracks
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == trackId);

            if (track == null)
                return new RemoveTrackResponse { Success = false, ErrorMessage = "Track not found" };

            if (track.AddedById != userId && track.Room.OwnerId != userId)
            {
                return new RemoveTrackResponse { Success = false, ErrorMessage = "Forbidden" };
            }

            context.Tracks.Remove(track);
            await context.SaveChangesAsync();

            await bus.PublishAsync(new TrackRemovedEvent(track.RoomId, track.Id));
            
            return new RemoveTrackResponse { Success = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing track");
            return new RemoveTrackResponse { Success = false, ErrorMessage = ex.Message };
        }
    }

    public override async Task<PlayerResponse> Pause(PlayerRequest request, ServerCallContext callContext)
    {
        var roomId = Guid.Parse(request.RoomId);

        var currentTrack = await context.Tracks
            .FirstOrDefaultAsync(t => t.RoomId == roomId && t.Status == TrackStatus.Playing);

        if (currentTrack == null)
            return new PlayerResponse { Success = false, ErrorMessage = "Nothing is playing right now" };

        var now = DateTimeOffset.UtcNow;
        var playedSoFar = (int)(now - (currentTrack.StartedAt ?? now)).TotalSeconds;

        currentTrack.PausedDurationInSeconds = playedSoFar;
        currentTrack.Status = TrackStatus.Paused;

        await context.SaveChangesAsync();
        orchestrator.CancelTimer(roomId);
       
        await bus.PublishAsync(new PlaybackStateChangedEvent(
            currentTrack.RoomId,
            currentTrack.Id,
            TrackStatus.Paused,
            currentTrack.PausedDurationInSeconds
        ));

        return new PlayerResponse { Success = true, Status = "Paused", CurrentTrackId = currentTrack.Id.ToString() };
    }

    public override async Task<PlayerResponse> Play(PlayerRequest request, ServerCallContext callContext)
    {
        var roomId = Guid.Parse(request.RoomId);

        var pausedTrack = await context.Tracks
            .FirstOrDefaultAsync(t => t.RoomId == roomId && t.Status == TrackStatus.Paused);

        if (pausedTrack == null)
        {
            return await Skip(new PlayerRequest { RoomId = request.RoomId, UserId = request.UserId }, callContext);
        }

        pausedTrack.StartedAt = DateTimeOffset.UtcNow.AddSeconds(-pausedTrack.PausedDurationInSeconds);
        pausedTrack.Status = TrackStatus.Playing;

        await context.SaveChangesAsync();

        await bus.PublishAsync(new PlaybackStateChangedEvent(
            pausedTrack.RoomId,
            pausedTrack.Id,
            TrackStatus.Playing,
            pausedTrack.PausedDurationInSeconds 
        ));
        
        var remainingSeconds = pausedTrack.DurationInSeconds - pausedTrack.PausedDurationInSeconds;

        orchestrator.ScheduleNextTrack(
            roomId,
            pausedTrack.Id,
            TimeSpan.FromSeconds(remainingSeconds)
        );

        return new PlayerResponse { Success = true, Status = "Playing", CurrentTrackId = pausedTrack.Id.ToString() };
    }

    public override async Task<PlayerResponse> Skip(PlayerRequest request, ServerCallContext callContext)
    {
        var roomId = Guid.Parse(request.RoomId);

        var currentTrack = await context.Tracks
            .FirstOrDefaultAsync(t =>
                t.RoomId == roomId && (t.Status == TrackStatus.Playing || t.Status == TrackStatus.Paused));

        if (currentTrack != null)
        {
            currentTrack.Status = TrackStatus.Played;
        }

        var nextTrack = await context.Tracks
            .Where(t => t.RoomId == roomId && t.Status == TrackStatus.Pending)
            .OrderByDescending(t => t.Score)
            .ThenBy(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        if (nextTrack == null)
            return new PlayerResponse { Success = false, ErrorMessage = "Playlist is empty", Status = "Stopped" };

        nextTrack.Status = TrackStatus.Playing;
        nextTrack.StartedAt = DateTimeOffset.UtcNow;
        nextTrack.PausedDurationInSeconds = 0;

        await context.SaveChangesAsync();

        orchestrator.ScheduleNextTrack(
            roomId,
            nextTrack.Id,
            TimeSpan.FromSeconds(nextTrack.DurationInSeconds)
        );
        
        await bus.PublishAsync(new PlaybackStateChangedEvent(
            roomId,
            nextTrack.Id,
            TrackStatus.Playing,
            0
        ));

        return new PlayerResponse { Success = true, Status = "Playing", CurrentTrackId = nextTrack.Id.ToString() };
    }
}