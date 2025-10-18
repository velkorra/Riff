using Riff.Api.Contracts.Dto;

namespace Riff.ApiGateway.Services.Interfaces;

public interface IRoomService
{
    Task<RoomResponse> Create(CreateRoomRequest request, Guid ownerId);
    Task<RoomResponse> GetById(Guid id);
}
