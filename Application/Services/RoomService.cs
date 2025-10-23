using Application.Mappings;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Application.Services;

public class RoomService : IRoomService
{
    private readonly RiffContext _context;

    public RoomService(RiffContext context)
    {
        _context = context;
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, Guid ownerId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == ownerId))
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

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        
        return room.ToDto();
    }

    public async Task<RoomResponse> GetByIdAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is null)
        {
            throw new ResourceNotFoundException(nameof(Room), id);
        }
        return room.ToDto();
    }
    
    public async Task<IEnumerable<RoomResponse>> GetRoomsByOwnerIdAsync(Guid ownerId)
    {
        var rooms = await _context.Rooms
            .Where(r => r.OwnerId == ownerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return rooms.ToDto();
    }
}