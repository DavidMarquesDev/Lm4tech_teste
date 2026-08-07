using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Infrastructure.Persistence;
using ProductChallenge.Infrastructure.Repositories;

namespace ProductChallenge.Infrastructure;

/// <summary>
/// Concentra aqui o registro e a migração para que a camada de apresentação não precise
/// referenciar o Entity Framework apenas para se compor.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }

    public static void MigrateDatabase(this IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var factory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var context = factory.CreateDbContext();

        context.Database.Migrate();
    }
}
