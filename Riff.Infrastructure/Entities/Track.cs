using System.ComponentModel.DataAnnotations;
using Riff.Api.Contracts.Enums;
using Riff.Infrastructure.Interfaces;

namespace Riff.Infrastructure.Entities;

public class Track : IAuditable
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(200)]
    public string Artist { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int DurationInSeconds { get; set; }
    public TrackStatus Status { get; set; } = TrackStatus.Pending;
    public int Score { get; set; } = 0;
    public int PausedDurationInSeconds { get; set; } = 0;
    public Guid AddedById { get; set; }
    public User AddedBy { get; set; } = null!;
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}