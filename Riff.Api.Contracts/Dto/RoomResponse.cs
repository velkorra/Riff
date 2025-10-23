namespace Riff.Api.Contracts.Dto;

public record RoomResponse(
    Guid Id,
    string Name,
    Guid OwnerId,
    DateTimeOffset CreatedAt
)
{
    public List<LinkDto> Links { get; set; } = [];
}