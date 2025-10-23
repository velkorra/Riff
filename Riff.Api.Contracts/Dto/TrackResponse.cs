namespace Riff.Api.Contracts.Dto;

public record TrackResponse(
    Guid Id,
    string Title,
    string Artist,
    string Url,
    int DurationInSeconds,
    DateTimeOffset AddedAt,
    Guid AddedById,
    Guid RoomId
)
{
    public List<LinkDto> Links { get; set; } = [];
}