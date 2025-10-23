using Microsoft.EntityFrameworkCore;
using Riff.Infrastructure.Entities;

namespace Riff.Infrastructure;

public class RiffContext: DbContext
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Track> Tracks { get; set; }

    public RiffContext(DbContextOptions<RiffContext> options) : base(options)
    {
    }
}
