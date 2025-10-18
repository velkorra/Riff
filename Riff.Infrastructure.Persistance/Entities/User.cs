using System.ComponentModel.DataAnnotations;

namespace Riff.Infrastructure.Persistance.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string PasswordHash { get; set; }
    public string? Username { get; set; }


}
