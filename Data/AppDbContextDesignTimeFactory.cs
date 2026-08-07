using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductChallenge.Data;

/// <summary>
/// Necessária porque uma aplicação Windows Forms não expõe um host que permita às ferramentas
/// do EF descobrir o DbContext em tempo de design.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(DatabaseLocation.ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}
