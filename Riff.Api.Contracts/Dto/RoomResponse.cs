namespace Riff.Api.Contracts.Dto;

public record RoomResponse(
Guid Id,
string Name,
Guid OwnerId,
DateTime CreatedAt
);

