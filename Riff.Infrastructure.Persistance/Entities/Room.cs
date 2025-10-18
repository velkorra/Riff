using System.ComponentModel.DataAnnotations;

namespace Riff.Infrastructure.Persistance.Entities;

public class Room
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public User Owner { get; set; } = null!;
}
