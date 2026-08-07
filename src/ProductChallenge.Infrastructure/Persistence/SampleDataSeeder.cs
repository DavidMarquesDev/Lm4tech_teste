using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductChallenge.Domain;

namespace ProductChallenge.Infrastructure.Persistence;

/// <summary>
/// Massa de demonstração, acionada apenas pelo argumento <c>--seed</c>. Não roda sozinha para
/// que uma execução normal não misture dados fictícios com os do usuário.
/// </summary>
public static class SampleDataSeeder
{
    private static readonly (ProductCategory Category, string[] Names)[] Catalog =
    [
        (ProductCategory.Electronics,
            ["Monitor", "Teclado mecânico", "Mouse sem fio", "Headset", "Webcam", "SSD NVMe", "Roteador"]),
        (ProductCategory.Groceries,
            ["Arroz integral", "Café torrado", "Azeite extra virgem", "Macarrão grano duro", "Açúcar cristal"]),
        (ProductCategory.Apparel,
            ["Camiseta algodão", "Calça jeans", "Jaqueta corta-vento", "Tênis running", "Meia esportiva"]),
        (ProductCategory.HomeAndGarden,
            ["Cafeteira", "Panela de pressão", "Jogo de lençol", "Luminária de mesa", "Aspirador vertical"]),
        (ProductCategory.Toys,
            ["Quebra-cabeça", "Carrinho de controle", "Boneca de pano", "Jogo de tabuleiro", "Blocos de montar"]),
    ];

    private static readonly string[] Lines =
        ["Alpha", "Bravo", "Cosmo", "Delta", "Élan", "Forte", "Giro", "Haven", "Íris", "Jade"];

    /// <summary>
    /// Só grava se a tabela estiver vazia, para que repetir o comando não duplique o catálogo.
    /// </summary>
    /// <returns>Quantidade gravada, ou zero se já havia dados.</returns>
    public static int SeedSampleData(this IServiceProvider provider, int quantity)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var factory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var context = factory.CreateDbContext();

        if (context.Products.Any())
        {
            return 0;
        }

        var random = new Random(Seed: quantity);

        for (var index = 1; index <= quantity; index++)
        {
            var (category, names) = Catalog[index % Catalog.Length];
            var name = $"{names[random.Next(names.Length)]} {Lines[random.Next(Lines.Length)]} {index:D3}";
            var price = Math.Round((decimal)((random.NextDouble() * 4800) + 9.9), 2);

            context.Products.Add(Product.Create(
                name,
                $"Item de demonstração {index}. Lote {random.Next(1000, 9999)}.",
                price,
                category,
                random.Next(0, 500)));
        }

        context.SaveChanges();

        return quantity;
    }
}
