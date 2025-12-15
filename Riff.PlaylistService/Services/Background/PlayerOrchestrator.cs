using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Enums;
using Riff.Infrastructure;

namespace Riff.PlaylistService.Services.Background;

public class PlayerOrchestrator(IServiceScopeFactory scopeFactory, ILogger<PlayerOrchestrator> logger)
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeTimers = new();

    public void ScheduleNextTrack(Guid roomId, Guid trackId, TimeSpan delay)
    {
        CancelTimer(roomId);

        var cts = new CancellationTokenSource();
        _activeTimers[roomId] = cts;

        logger.LogInformation(
            $"[Timer] Started timer for room {roomId}. Track {trackId} will finish in {delay.TotalSeconds} seconds.");

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, cts.Token);

                await SwitchToNextTrackAsync(roomId, trackId);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation($"[Timer] Timer for room {roomId} was canceled (Pause or Skip).");
            }
        }, cts.Token);
    }

    public void CancelTimer(Guid roomId)
    {
        if (_activeTimers.TryRemove(roomId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    private async Task SwitchToNextTrackAsync(Guid roomId, Guid finishedTrackId)
    {
        logger.LogInformation($"[Timer] Time is up for track {finishedTrackId}. Switching to next track...");

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RiffContext>();

        var currentTrack = await context.Tracks.FindAsync(finishedTrackId);
        if (currentTrack != null && currentTrack.Status == TrackStatus.Playing)
        {
            currentTrack.Status = TrackStatus.Played;
        }

        var nextTrack = await context.Tracks
            .Where(t => t.RoomId == roomId && t.Status == TrackStatus.Pending)
            .OrderByDescending(t => t.Score)
            .ThenBy(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        if (nextTrack != null)
        {
            nextTrack.Status = TrackStatus.Playing;
            nextTrack.StartedAt = DateTimeOffset.UtcNow;
            nextTrack.PausedDurationInSeconds = 0;

            await context.SaveChangesAsync();

            ScheduleNextTrack(roomId, nextTrack.Id, TimeSpan.FromSeconds(nextTrack.DurationInSeconds));
        }
        else
        {
            await context.SaveChangesAsync();
            logger.LogInformation($"[Timer] Playlist for room {roomId} has ended.");
        }
    }
}