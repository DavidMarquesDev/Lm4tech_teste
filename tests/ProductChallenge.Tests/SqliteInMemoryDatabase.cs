using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProductChallenge.Data;

namespace ProductChallenge.Tests;

/// <summary>
/// Usa o provider real do SQLite, e não o InMemory do EF, que não suporta ExecuteDelete nem
/// aplica as restrições do mapeamento. A conexão fica aberta porque o SQLite descarta o banco
/// em memória ao fechar a última conexão.
/// </summary>
public sealed class SqliteInMemoryDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public SqliteInMemoryDatabase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.Migrate();
    }

    public AppDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
