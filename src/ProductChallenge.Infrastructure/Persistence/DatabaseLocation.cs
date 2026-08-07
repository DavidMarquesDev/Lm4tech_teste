namespace ProductChallenge.Infrastructure.Persistence;

public static class DatabaseLocation
{
    private const string FileName = "products.db";

    /// <summary>
    /// Absoluto de propósito: um caminho relativo dependeria do diretório de trabalho, que
    /// difere entre a execução pelo Visual Studio e pela linha de comando.
    /// </summary>
    public static string ConnectionString =>
        $"Data Source={Path.Combine(AppContext.BaseDirectory, FileName)}";
}
