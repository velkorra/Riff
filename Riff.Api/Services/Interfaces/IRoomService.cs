using Riff.Api.Contracts.Dto;

namespace Riff.Api.Services.Interfaces;

public interface IRoomService
{
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, Guid ownerId);
    Task<RoomResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<RoomResponse>> GetRoomsByOwnerIdAsync(Guid ownerId);
    Task<IEnumerable<RoomResponse>> GetPublicRoomsAsync(int limit = 20);
}