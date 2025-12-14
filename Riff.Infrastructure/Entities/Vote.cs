namespace Riff.Infrastructure.Entities;

public class Vote
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid TrackId { get; set; }
    public Track Track { get; set; } = null!;

    public int Value { get; set; } 
}