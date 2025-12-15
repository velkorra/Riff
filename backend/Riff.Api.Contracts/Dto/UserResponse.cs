namespace Riff.Api.Contracts.Dto;

public record UserResponse(
    Guid Id,
    string Username
)
{
    public List<LinkDto> Links { get; set; } = [];
}