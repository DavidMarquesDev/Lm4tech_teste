using Microsoft.Extensions.DependencyInjection;
using ProductChallenge.Application.Abstractions;
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
        services.AddInfrastructure(connectionString);

        // Serviço e repositório não guardam estado e criam um contexto por operação, então
        // Transient dentro de um consumidor de vida longa não retém dados obsoletos.
        services.AddTransient<IProductService, ProductService>();

        services.AddSingleton<ProductListViewModel>();
        services.AddSingleton<MainForm>();

        return services;
    }
}
