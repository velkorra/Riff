using System.ComponentModel.DataAnnotations;

namespace Riff.Api.Contracts.Dto;
public record CreateRoomRequest(
[Required]
[StringLength(100, MinimumLength = 3, ErrorMessage = "The room name must be between 3 and 100 characters.")]
string Name,
string? Password
);
