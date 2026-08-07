namespace ProductChallenge.Application.Reporting;

public interface IProductExportService
{
    IReadOnlyList<ExportField> GetAvailableFields();

    /// <summary>
    /// Exporta o que corresponde ao filtro em uso, e não o catálogo inteiro: quem reduziu a
    /// lista a doze itens espera receber doze.
    /// </summary>
    /// <returns>Quantidade de linhas gravadas.</returns>
    Task<int> ExportAsync(IReadOnlyList<string> fieldNames, string searchTerm, Stream destination);
}
