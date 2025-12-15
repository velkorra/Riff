using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Riff.Infrastructure;

public class RiffContextFactory : IDesignTimeDbContextFactory<RiffContext>
{
    public RiffContext CreateDbContext(string[] args)
    {
        string? connectionString = null;
        for (var i = 0; i < args.Length; i++)
        {
            if ((args[i] == "-c" || args[i] == "--connection") && i + 1 < args.Length)
            {
                connectionString = args[i + 1];
                break;
            }
        }

        var optionsBuilder = new DbContextOptionsBuilder<RiffContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new RiffContext(optionsBuilder.Options);
    }
}