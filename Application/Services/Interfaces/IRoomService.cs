using Riff.Api.Contracts.Dto;

namespace Application.Services.Interfaces;

public interface IRoomService
{
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, Guid ownerId);
    Task<RoomResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<RoomResponse>> GetRoomsByOwnerIdAsync(Guid ownerId);
}