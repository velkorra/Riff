using Application.Mappings;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Application.Services;

public class TrackService : ITrackService
{
    private readonly RiffContext _context;

    public TrackService(RiffContext context)
    {
        _context = context;
    }
    
    public async Task<TrackResponse> AddTrackAsync(Guid roomId, AddTrackRequest request, Guid userId)
    {
        if (!await _context.Rooms.AnyAsync(r => r.Id == roomId))
        {
            throw new ResourceNotFoundException(nameof(Room), roomId);
        }
        
        var track = new Track
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Artist = request.Artist,
            Url = request.Url,
            DurationInSeconds = request.DurationInSeconds,
            RoomId = roomId,
            AddedById = userId,
            AddedAt = DateTime.UtcNow
        };

        await _context.Tracks.AddAsync(track);
        await _context.SaveChangesAsync();

        return track.ToDto();
    }

    public async Task<IEnumerable<TrackResponse>> GetPlaylistAsync(Guid roomId)
    {
        if (!await _context.Rooms.AnyAsync(r => r.Id == roomId))
        {
            throw new ResourceNotFoundException(nameof(Room), roomId);
        }
        
        var tracks = await _context.Tracks
            .Where(t => t.RoomId == roomId)
            .OrderBy(t => t.AddedAt)
            .ToListAsync();

        return tracks.ToDto();
    }

    public async Task DeleteTrackAsync(Guid trackId, Guid userId)
    {
        var track = await _context.Tracks
            .Include(t => t.Room)
            .FirstOrDefaultAsync(t => t.Id == trackId);
        
        if (track is null)
        {
            throw new ResourceNotFoundException(nameof(Track), trackId);
        }

        if (track.AddedById != userId && track.Room.OwnerId != userId)
        {
            // TODO: Create a specific ForbiddenAccessException
            throw new UnauthorizedAccessException("User is not authorized to delete this track.");
        }

        _context.Tracks.Remove(track);
        await _context.SaveChangesAsync();
    }
}