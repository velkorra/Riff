using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Riff.Infrastructure.Entities;

namespace Riff.Infrastructure;

public class RiffContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IDataProtectionKeyContext
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public RiffContext(DbContextOptions<RiffContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vote>()
            .HasKey(v => new { v.UserId, v.TrackId });


        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Track)
            .WithMany()
            .HasForeignKey(v => v.TrackId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}