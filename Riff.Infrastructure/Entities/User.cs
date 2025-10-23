using System.ComponentModel.DataAnnotations;

namespace Riff.Infrastructure.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public List<Room> OwnedRooms { get; set; } = [];

    public List<Track> AddedTracks { get; set; } = [];
}