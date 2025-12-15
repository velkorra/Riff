using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Api.Mappings;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Riff.Api.Services;

public class TrackService(RiffContext context) : ITrackService
{
    public async Task<IEnumerable<TrackResponse>> GetPlaylistAsync(Guid roomId)
    {
        if (!await context.Rooms.AnyAsync(r => r.Id == roomId))
        {
            throw new ResourceNotFoundException(nameof(Room), roomId);
        }

        var tracks = await context.Tracks
            .Where(t => t.RoomId == roomId)
            .OrderByDescending(t => t.Score)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();

        return tracks.ToDto();
    }

    public async Task<TrackResponse> GetByIdAsync(Guid trackId)
    {
        var track = await context.Tracks
            .Include(t => t.Room)
            .FirstOrDefaultAsync(t => t.Id == trackId);

        if (track is null)
        {
            throw new ResourceNotFoundException(nameof(Track), trackId);
        }

        return track.ToDto();
    }

    public async Task<IEnumerable<TrackResponse>> GetGlobalTopAsync(int limit = 20)
    {
        var tracks = await context.Tracks
            .OrderByDescending(t => t.Score)
            .ThenByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return tracks.ToDto();
    }
}