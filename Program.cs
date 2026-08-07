using Microsoft.EntityFrameworkCore;
using ProductChallenge.Data;
using ProductChallenge.ViewModels;
using ProductChallenge.Views;

namespace ProductChallenge;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // DbContextOptions é imutável e pode ser compartilhado entre contextos.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(DatabaseLocation.ConnectionString)
            .Options;

        AppDbContext CreateContext() => new(options);

        using (var context = CreateContext())
        {
            context.Database.Migrate();
        }

        Application.Run(new MainForm(new ProductListViewModel(CreateContext)));
    }
}
