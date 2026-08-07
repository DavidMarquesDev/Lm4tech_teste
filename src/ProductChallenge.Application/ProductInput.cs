using ProductChallenge.Domain;
using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Application;

/// <summary>
/// O que o usuário preencheu. Os tipos são anuláveis para que "não informado" seja distinguível
/// do zero, e as regras vivem em atributos lidos pelo validador em vez de espalhadas em ifs.
/// </summary>
public sealed class ProductInput
{
    [Required("Informe o nome do produto.")]
    [TextLength(Min = 2, Max = Product.NameMaxLength,
        ErrorMessage = "O nome deve ter entre 2 e 120 caracteres.")]
    public string? Name { get; set; }

    [TextLength(Max = Product.DescriptionMaxLength,
        ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Description { get; set; }

    [Required("Informe o preço.")]
    [NumberRange(Min = 0.01, Max = 1_000_000,
        ErrorMessage = "O preço deve ser maior que zero e no máximo 1.000.000,00.")]
    public decimal? Price { get; set; }

    [Required("Selecione uma categoria.")]
    public ProductCategory? Category { get; set; }

    [Required("Informe a quantidade em estoque.")]
    [NumberRange(Min = 0, Max = Product.StockMaxValue,
        ErrorMessage = "O estoque deve estar entre 0 e 1.000.000.")]
    public int? StockQuantity { get; set; }

    /// <summary>Só deve ser chamado depois de o validador aprovar a entrada.</summary>
    public ProductDraft ToDraft() => new(
        Name!.Trim(),
        string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
        Price!.Value,
        Category!.Value,
        StockQuantity!.Value);
}
