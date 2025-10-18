using Microsoft.EntityFrameworkCore;
using Riff.Infrastructure.Persistance.Entities;

namespace Riff.Infrastructure.Persistance;

public class RiffContext: DbContext
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }

    public RiffContext(DbContextOptions<RiffContext> options) : base(options)
    {
    }
}
