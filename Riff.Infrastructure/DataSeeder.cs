using Microsoft.AspNetCore.Identity;
using Riff.Infrastructure.Entities;

namespace Riff.Infrastructure;

public static class DataSeeder
{
    public static async Task InitializeDatabase(RiffContext context, UserManager<User> userManager)
    {
        if (context.Users.Any())
        {
            return;
        }

        var userGura = new User
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6"),
            UserName = "gura_chan",
            Email = "gura@holo.com",
        };

        var userLily = new User
        {
            Id = Guid.Parse("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6"),
            UserName = "shylily",
            Email = "shylily@holo.com",
        };

        await userManager.CreateAsync(userGura, "@123Abc");
        await userManager.CreateAsync(userLily, "@123Abc");


        var roomCityPop = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Gura's City Pop & 80s Hits",
            OwnerId = userGura.Id,
            PasswordHash = null,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-2)
        };

        var roomVKei = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Lily's Visual Kei Sanctuary",
            OwnerId = userLily.Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("diru_forever"),
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var tracks = new List<Track>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Plastic Love",
                Artist = "Mariya Takeuchi",
                Url = "https://www.youtube.com/watch?v=3bNITQR4Uso",
                DurationInSeconds = 478,
                RoomId = roomCityPop.Id,
                AddedById = userGura.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Last Summer Whisper",
                Artist = "Anri",
                Url = "https://www.youtube.com/watch?v=4p1I2G-kG-M",
                DurationInSeconds = 309,
                RoomId = roomCityPop.Id,
                AddedById = userLily.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-9)
            },

            new()
            {
                Id = Guid.NewGuid(),
                Title = "Obscure",
                Artist = "DIR EN GREY",
                Url = "https://www.youtube.com/watch?v=buOF-1I32yY",
                DurationInSeconds = 239,
                RoomId = roomVKei.Id,
                AddedById = userLily.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Filth in the Beauty",
                Artist = "The Gazette",
                Url = "https://www.youtube.com/watch?v=r02k_b_3p_0",
                DurationInSeconds = 312,
                RoomId = roomVKei.Id,
                AddedById = userLily.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-4)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Beast of Blood",
                Artist = "Malice Mizer",
                Url = "https://www.youtube.com/watch?v=x1wgD3bNLqI&list=RDx1wgD3bNLqI",
                DurationInSeconds = 308,
                RoomId = roomVKei.Id,
                AddedById = userGura.Id,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-3)
            }
        };

        // context.Users.AddRange(userGura, userLily);
        context.Rooms.AddRange(roomCityPop, roomVKei);
        context.Tracks.AddRange(tracks);

        context.SaveChanges();
    }
}