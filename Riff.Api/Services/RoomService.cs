using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.ApiGateway.Mappings;
using Riff.ApiGateway.Services.Interfaces;
using Riff.Infrastructure.Persistance;
using Riff.Infrastructure.Persistance.Entities;

namespace Riff.ApiGateway.Services;

public class RoomService : IRoomService
{
    private readonly RiffContext _context;

    public RoomService(RiffContext context)
    {
        _context = context;
    }
    public async Task<RoomResponse> Create(CreateRoomRequest request, Guid ownerId)
    {
        if (request.Password is not null && string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password cannot be empty or whitespace.", nameof(request.Password));
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

    public async Task<RoomResponse> GetById(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room is null)
        {
            throw new ResourceNotFoundException(nameof(Room), id);
        }
        return room.ToDto();
    }
}
