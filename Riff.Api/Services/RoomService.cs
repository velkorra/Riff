using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Api.Mappings;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Riff.Api.Services;

public class RoomService(RiffContext context) : IRoomService
{
    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, Guid ownerId)
    {
        if (!await context.Users.AnyAsync(u => u.Id == ownerId))
        {
            throw new ResourceNotFoundException(nameof(User), ownerId);
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = request.Password is not null 
                ? BCrypt.Net.BCrypt.HashPassword(request.Password) 
                : null
        };

        await context.Rooms.AddAsync(room);
        await context.SaveChangesAsync();
        
        return room.ToDto();
    }

    public async Task<RoomResponse> GetByIdAsync(Guid id)
    {
        var room = await context.Rooms.FindAsync(id);
        if (room is null)
        {
            throw new ResourceNotFoundException(nameof(Room), id);
        }
        return room.ToDto();
    }
    
    public async Task<IEnumerable<RoomResponse>> GetRoomsByOwnerIdAsync(Guid ownerId)
    {
        var rooms = await context.Rooms
            .Where(r => r.OwnerId == ownerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return rooms.ToDto();
    }
    
    public async Task<IEnumerable<RoomResponse>> GetPublicRoomsAsync(int limit = 20)
    {
        var rooms = await context.Rooms
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return rooms.ToDto();
    }
}