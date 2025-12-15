using System.ComponentModel.DataAnnotations;

namespace Riff.Api.Contracts.Dto;

public record AddTrackRequest(
    [Required]
    [StringLength(200)]
    string Title,
    [Required]
    [StringLength(200)]
    string Artist,
    [Required]
    [Url]
    string Url,
    [Range(1, 3600)]
    int DurationInSeconds
);