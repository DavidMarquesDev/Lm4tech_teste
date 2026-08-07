using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Domain;

public class Product
{
    public const int NameMaxLength = 120;
    public const int DescriptionMaxLength = 1000;
    public const decimal PriceMaxValue = 1_000_000m;
    public const int StockMaxValue = 1_000_000;

    [ExportColumn("Código", 1)]
    public int Id { get; private set; }

    [ExportColumn("Produto", 2)]
    public string Name { get; private set; } = string.Empty;

    [ExportColumn("Descrição", 6)]
    public string? Description { get; private set; }

    [ExportColumn("Preço", 4, Format = "N2")]
    public decimal Price { get; private set; }

    [ExportColumn("Categoria", 3)]
    public ProductCategory Category { get; private set; }

    [ExportColumn("Estoque", 5)]
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Coluna derivada de nome e descrição. Existe porque o LIKE do SQLite só ignora caixa para
    /// ASCII e não respeita collation, então "eletronico" não encontraria "Eletrônico".
    /// </summary>
    public string SearchText { get; private set; } = string.Empty;

    public const int SearchTextMaxLength = NameMaxLength + DescriptionMaxLength + 1;

    private Product()
    {
    }

    public static Product Create(
        string name, string? description, decimal price, ProductCategory category, int stockQuantity)
    {
        var product = new Product();
        product.SetDetails(name, description, price, category, stockQuantity);
        return product;
    }

    /// <summary>
    /// Rede de segurança do domínio. As mensagens amigáveis vêm antes, na apresentação: uma
    /// exceção aqui indica regra não aplicada na entrada, não erro do usuário.
    /// </summary>
    public void SetDetails(
        string name, string? description, decimal price, ProductCategory category, int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (normalizedName.Length > NameMaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(name),
                $"O nome deve ter no máximo {NameMaxLength} caracteres.");
        }

        if (normalizedDescription?.Length > DescriptionMaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(description),
                $"A descrição deve ter no máximo {DescriptionMaxLength} caracteres.");
        }

        if (price <= 0m || price > PriceMaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                $"O preço deve ser maior que zero e no máximo {PriceMaxValue:N2}.");
        }

        if (stockQuantity is < 0 or > StockMaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stockQuantity),
                $"O estoque deve estar entre 0 e {StockMaxValue}.");
        }

        if (!Enum.IsDefined(category))
        {
            throw new ArgumentOutOfRangeException(nameof(category), "Categoria inválida.");
        }

        Name = normalizedName;
        Description = normalizedDescription;
        Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        Category = category;
        StockQuantity = stockQuantity;
        SearchText = SearchNormalizer.Normalize($"{normalizedName} {normalizedDescription}");
    }
}
