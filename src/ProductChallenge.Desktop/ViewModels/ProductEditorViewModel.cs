using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using ProductChallenge.Application;
using ProductChallenge.Domain;
using ProductChallenge.Domain.Validation;

namespace ProductChallenge.Desktop.ViewModels;

/// <summary>
/// Preço e estoque são texto porque o painel precisa refletir exatamente o que foi digitado,
/// inclusive quando o conteúdo ainda não é um número válido.
/// </summary>
public partial class ProductEditorViewModel : ObservableObject
{
    private const NumberStyles PriceStyles =
        NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditing))]
    private int? _editingId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _price = string.Empty;

    [ObservableProperty]
    private string _stockQuantity = string.Empty;

    [ObservableProperty]
    private CategoryOption? _selectedCategory;

    [ObservableProperty]
    private IReadOnlyList<ValidationFailure> _errors = [];

    public IReadOnlyList<CategoryOption> Categories => ProductCategoryCatalog.Options;

    public bool IsEditing => EditingId.HasValue;

    public void StartNew()
    {
        EditingId = null;
        Name = string.Empty;
        Description = string.Empty;
        Price = string.Empty;
        StockQuantity = string.Empty;
        SelectedCategory = null;
        Errors = [];
    }

    public void StartEdit(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        EditingId = product.Id;
        Name = product.Name;
        Description = product.Description ?? string.Empty;
        Price = product.Price.ToString("N2", CultureInfo.CurrentCulture);
        StockQuantity = product.StockQuantity.ToString(CultureInfo.CurrentCulture);
        SelectedCategory = ProductCategoryCatalog.Find(product.Category);
        Errors = [];
    }

    /// <summary>
    /// Devolve <c>null</c> quando há erros, que ficam publicados em <see cref="Errors"/> para
    /// a View associar a cada controle.
    /// </summary>
    public ProductDraft? TryBuildDraft()
    {
        // A conversão de texto para número não é expressável como atributo, então fica aqui.
        // Obrigatoriedade, faixa e tamanho ficam nos atributos de ProductInput.
        var parseFailures = new List<ValidationFailure>();

        var input = new ProductInput
        {
            Name = Name,
            Description = Description,
            Category = SelectedCategory?.Category,
            Price = ParsePrice(parseFailures),
            StockQuantity = ParseStockQuantity(parseFailures)
        };

        var failures = new List<ValidationFailure>(parseFailures);

        failures.AddRange(DynamicValidator.Validate(input).Failures
            .Where(failure => parseFailures.TrueForAll(
                parsed => parsed.PropertyName != failure.PropertyName)));

        Errors = failures;

        return failures.Count == 0 ? input.ToDraft() : null;
    }

    private decimal? ParsePrice(List<ValidationFailure> failures)
    {
        if (string.IsNullOrWhiteSpace(Price))
        {
            return null;
        }

        if (decimal.TryParse(Price, PriceStyles, CultureInfo.CurrentCulture, out var price))
        {
            return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        }

        failures.Add(new ValidationFailure(
            nameof(ProductInput.Price), "Preço inválido. Informe apenas números."));

        return null;
    }

    private int? ParseStockQuantity(List<ValidationFailure> failures)
    {
        if (string.IsNullOrWhiteSpace(StockQuantity))
        {
            return null;
        }

        if (int.TryParse(StockQuantity, NumberStyles.Integer, CultureInfo.CurrentCulture, out var stock))
        {
            return stock;
        }

        failures.Add(new ValidationFailure(
            nameof(ProductInput.StockQuantity), "Estoque inválido. Informe um número inteiro."));

        return null;
    }
}
