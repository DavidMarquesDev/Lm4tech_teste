namespace ProductChallenge.Application.Abstractions;

/// <summary>
/// Traduz falhas de persistência para um tipo que as camadas acima conseguem nomear sem
/// referenciar o Entity Framework.
/// </summary>
public sealed class DataAccessException : Exception
{
    public DataAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
