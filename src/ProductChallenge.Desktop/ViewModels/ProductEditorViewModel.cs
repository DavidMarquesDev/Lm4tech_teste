using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using ProductChallenge.Application;
using ProductChallenge.Desktop.Common;
using ProductChallenge.Domain;

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
    private IReadOnlyList<FieldError> _errors = [];

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
        var errors = new List<FieldError>();

        var name = Name.Trim();
        if (name.Length == 0)
        {
            errors.Add(new FieldError(nameof(Name), "Informe o nome do produto."));
        }
        else if (name.Length > Product.NameMaxLength)
        {
            errors.Add(new FieldError(
                nameof(Name),
                $"O nome deve ter no máximo {Product.NameMaxLength} caracteres."));
        }

        var description = Description.Trim();
        if (description.Length > Product.DescriptionMaxLength)
        {
            errors.Add(new FieldError(
                nameof(Description),
                $"A descrição deve ter no máximo {Product.DescriptionMaxLength} caracteres."));
        }

        var price = ValidatePrice(errors);
        var stock = ValidateStockQuantity(errors);

        if (SelectedCategory is null)
        {
            errors.Add(new FieldError(nameof(SelectedCategory), "Selecione uma categoria."));
        }

        Errors = errors;

        return errors.Count > 0
            ? null
            : new ProductDraft(
                name,
                description.Length == 0 ? null : description,
                price,
                SelectedCategory!.Category,
                stock);
    }

    private decimal ValidatePrice(List<FieldError> errors)
    {
        if (string.IsNullOrWhiteSpace(Price))
        {
            errors.Add(new FieldError(nameof(Price), "Informe o preço."));
            return 0m;
        }

        if (!decimal.TryParse(Price, PriceStyles, CultureInfo.CurrentCulture, out var price))
        {
            errors.Add(new FieldError(nameof(Price), "Preço inválido. Informe apenas números."));
            return 0m;
        }

        if (price <= 0m)
        {
            errors.Add(new FieldError(nameof(Price), "O preço deve ser maior que zero."));
            return 0m;
        }

        if (price > Product.PriceMaxValue)
        {
            errors.Add(new FieldError(
                nameof(Price),
                $"O preço não pode passar de {Product.PriceMaxValue.ToString("N2", CultureInfo.CurrentCulture)}."));
            return 0m;
        }

        return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }

    private int ValidateStockQuantity(List<FieldError> errors)
    {
        if (string.IsNullOrWhiteSpace(StockQuantity))
        {
            errors.Add(new FieldError(nameof(StockQuantity), "Informe a quantidade em estoque."));
            return 0;
        }

        if (!int.TryParse(StockQuantity, NumberStyles.Integer, CultureInfo.CurrentCulture, out var stock))
        {
            errors.Add(new FieldError(nameof(StockQuantity), "Estoque inválido. Informe um número inteiro."));
            return 0;
        }

        if (stock < 0)
        {
            errors.Add(new FieldError(nameof(StockQuantity), "O estoque não pode ser negativo."));
            return 0;
        }

        if (stock > Product.StockMaxValue)
        {
            errors.Add(new FieldError(
                nameof(StockQuantity),
                $"O estoque não pode passar de {Product.StockMaxValue.ToString("N0", CultureInfo.CurrentCulture)}."));
            return 0;
        }

        return stock;
    }
}
