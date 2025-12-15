using System.ComponentModel.DataAnnotations;

namespace Riff.Api.Contracts.Dto;

public record RegisterUserRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    string Username,
    [Required]
    [StringLength(100, MinimumLength = 8)]
    string Password
);