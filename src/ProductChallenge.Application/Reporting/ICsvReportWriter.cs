namespace ProductChallenge.Application.Reporting;

public interface ICsvReportWriter
{
    /// <summary>Genérico porque a escrita não depende de produto: recebe linhas e colunas.</summary>
    Task WriteAsync<T>(IEnumerable<T> rows, IReadOnlyList<ExportField> fields, Stream destination)
        where T : notnull;
}
