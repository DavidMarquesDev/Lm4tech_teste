using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Application.Reporting;
using ProductChallenge.Application.Services;
using ProductChallenge.Desktop.ViewModels;
using ProductChallenge.Desktop.Views;
using ProductChallenge.Infrastructure;

namespace ProductChallenge.Desktop.Composition;

internal static class ServiceRegistration
{
    public static IServiceCollection AddProductChallenge(
        this IServiceCollection services, string connectionString)
    {
        // Arquivo em vez de console: num WinExe não há console anexado, e a saída seria
        // descartada em silêncio. O AddDebug complementa, mas só entrega com depurador anexado.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.File(
                Path.Combine(LogLocation.Directory, "produtos-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:l}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddDebug();
            builder.AddSerilog();
        });

        services.AddInfrastructure(connectionString);

        // Serviço e repositório não guardam estado e criam um contexto por operação, então
        // Transient dentro de um consumidor de vida longa não retém dados obsoletos.
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IProductExportService, ProductExportService>();

        services.AddSingleton<ProductListViewModel>();
        services.AddSingleton<MainForm>();
        services.AddTransient<ExportColumnsDialog>();

        // Func<T> em vez de IServiceProvider: a dependência fica declarada no construtor da View,
        // em vez de escondida atrás de um service locator.
        services.AddTransient<Func<ExportColumnsDialog>>(
            provider => provider.GetRequiredService<ExportColumnsDialog>);

        return services;
    }
}
