using System.ComponentModel.DataAnnotations;

namespace Riff.Infrastructure.Entities;

public class Room
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public List<Track> Playlist { get; set; } = [];
}