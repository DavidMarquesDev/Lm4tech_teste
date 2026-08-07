namespace ProductChallenge.Desktop.Composition;

internal static class LogLocation
{
    /// <summary>
    /// Ao lado do executável, pelo mesmo motivo do banco: um caminho relativo dependeria do
    /// diretório de trabalho, que muda entre o Visual Studio e a linha de comando.
    /// </summary>
    public static string Directory { get; } = Path.Combine(AppContext.BaseDirectory, "logs");
}
