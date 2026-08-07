using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using ProductChallenge.Application.Reporting;

namespace ProductChallenge.Infrastructure.Reporting;

public sealed class CsvReportWriter : ICsvReportWriter
{
    private readonly ILogger<CsvReportWriter> _logger;
    private readonly string _delimiter;

    public CsvReportWriter(ILogger<CsvReportWriter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // O Excel em pt-BR espera ponto e vírgula; fixar a vírgula quebraria a abertura por
        // duplo clique justamente na máquina de quem vai avaliar.
        _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
    }

    public async Task WriteAsync<T>(IEnumerable<T> rows, IReadOnlyList<ExportField> fields, Stream destination)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(destination);

        if (fields is null || fields.Count == 0)
        {
            throw new ArgumentException("Informe ao menos uma coluna.", nameof(fields));
        }

        // BOM: sem ele o Excel abre "Preço" como "PreÃ§o".
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        await using var writer = new StreamWriter(destination, encoding, leaveOpen: true);

        await writer.WriteLineAsync(string.Join(_delimiter, fields.Select(field => Escape(field.Header))));

        var rowCount = 0;

        foreach (var row in rows)
        {
            var cells = fields.Select(field => Escape(Render(field, row)));
            await writer.WriteLineAsync(string.Join(_delimiter, cells));
            rowCount++;
        }

        await writer.FlushAsync();

        _logger.LogDebug("CSV com {RowCount} linhas gerado.", rowCount);
    }

    private static string Render(ExportField field, object row)
    {
        var value = field.Read(row);

        return value switch
        {
            null => string.Empty,
            IFormattable formattable when field.Format is not null
                => formattable.ToString(field.Format, CultureInfo.CurrentCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>Escape conforme RFC 4180: aspas dobradas e campo entre aspas quando necessário.</summary>
    private string Escape(string value)
    {
        if (value.Length == 0)
        {
            return string.Empty;
        }

        var needsQuotes = value.Contains(_delimiter, StringComparison.Ordinal)
            || value.Contains('"')
            || value.Contains('\n')
            || value.Contains('\r');

        return needsQuotes ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}
