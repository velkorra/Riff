using Riff.Infrastructure.Persistance.Entities;

namespace Riff.Infrastructure.Persistance;

public static class DataSeeder
{
    public static void InitializeDatabase(RiffContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var userAlice = new User
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
            Username = "Alice",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        var userBob = new User
        {
            Id = Guid.Parse("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6"),
            Username = "Bob",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("securepass")
        };

        var roomPublic = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Alice's Chill Zone",
            OwnerId = userAlice.Id,
            PasswordHash = null,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var roomPrivate = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Bob's Secret Lair",
            OwnerId = userBob.Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("topsecret"),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(userAlice, userBob);
        context.Rooms.AddRange(roomPublic, roomPrivate);

        context.SaveChanges();
    }
}