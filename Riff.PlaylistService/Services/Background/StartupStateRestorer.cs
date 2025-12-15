using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Enums;
using Riff.Infrastructure;

namespace Riff.PlaylistService.Services.Background;

public class StartupStateRestorer(
    IServiceScopeFactory scopeFactory,
    PlayerOrchestrator orchestrator,
    ILogger<StartupStateRestorer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(">>> STARTING PLAYER STATE RESTORE <<<");

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RiffContext>();

        var playingTracks = await context.Tracks
            .Where(t => t.Status == TrackStatus.Playing)
            .ToListAsync(stoppingToken);

        var now = DateTimeOffset.UtcNow;

        foreach (var track in playingTracks)
        {
            if (!track.StartedAt.HasValue) continue;

            var endTime = track.StartedAt.Value.AddSeconds(track.DurationInSeconds);
            var remaining = endTime - now;

            if (remaining.TotalSeconds <= 0)
            {
                logger.LogWarning($"Track {track.Title} expired ({remaining.TotalSeconds} sec). Skipping immediately.");

                orchestrator.ScheduleNextTrack(track.RoomId, track.Id, TimeSpan.Zero);
            }
            else
            {
                logger.LogInformation(
                    $"Track {track.Title} is active. {remaining.TotalSeconds} sec remaining. Restarting timer.");
                orchestrator.ScheduleNextTrack(track.RoomId, track.Id, remaining);
            }
        }
        
        logger.LogInformation($"Player restore completed. Processed {playingTracks.Count} tracks.");
    }
}