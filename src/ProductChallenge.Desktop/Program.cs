using Microsoft.Extensions.DependencyInjection;
using ProductChallenge.Desktop.Composition;
using ProductChallenge.Desktop.Views;
using ProductChallenge.Infrastructure;
using ProductChallenge.Infrastructure.Persistence;

namespace ProductChallenge.Desktop;

internal static class Program
{
    private const string SeedArgument = "--seed";
    private const int SampleSize = 100;

    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        services.AddProductChallenge(DatabaseLocation.ConnectionString);

        using var provider = services.BuildServiceProvider();
        provider.MigrateDatabase();

        if (args.Contains(SeedArgument, StringComparer.OrdinalIgnoreCase))
        {
            provider.SeedSampleData(SampleSize);
        }

        // Qualificado porque o namespace ProductChallenge.Application deixa "Application"
        // ambíguo neste escopo.
        System.Windows.Forms.Application.Run(provider.GetRequiredService<MainForm>());
    }
}
