namespace ProductChallenge.Domain.Metadata;

/// <summary>
/// Marca uma propriedade como exportável. É opt-in de propósito: colunas derivadas como
/// <see cref="Product.SearchText"/> não devem aparecer para o usuário escolher.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ExportColumnAttribute : Attribute
{
    public ExportColumnAttribute(string header, int order) => (Header, Order) = (header, order);

    public string Header { get; }

    public int Order { get; }

    /// <summary>Formato aplicado a valores <see cref="IFormattable"/>, como "N2" para moeda.</summary>
    public string? Format { get; set; }
}
