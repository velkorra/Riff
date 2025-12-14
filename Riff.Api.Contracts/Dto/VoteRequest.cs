using System.ComponentModel.DataAnnotations;

namespace Riff.Api.Contracts.Dto;

public record VoteRequest(
    [Required]
    [Range(-1, 1)]
    int Value
);