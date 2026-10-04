using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TwoGether.Persistence;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=twogether;Username=twogether;Password=twogether";

        var options = new DbContextOptionsBuilder<AppDbContext>();
        DependencyInjection.Configure(options, connectionString);
        return new AppDbContext(options.Options);
    }
}
