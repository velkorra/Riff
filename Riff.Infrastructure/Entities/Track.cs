using System.ComponentModel.DataAnnotations;

namespace Riff.Infrastructure.Entities;

public class Track
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(200)]
    public string Title { get; set; } = null!;
    
    [MaxLength(200)]
    public string Artist { get; set; } = null!;
    
    public string Url { get; set; } = null!;
    
    public int DurationInSeconds { get; set; }
    
    public DateTimeOffset AddedAt { get; set; }

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public Guid AddedById { get; set; }
    public User AddedBy { get; set; } = null!;
}