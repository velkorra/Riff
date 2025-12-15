using Microsoft.AspNetCore.Identity;
using Riff.Infrastructure.Interfaces;

namespace Riff.Infrastructure.Entities;

public class User : IdentityUser<Guid>, IAuditable
{
    public List<Room> OwnedRooms { get; set; } = [];
    public List<Track> AddedTracks { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}