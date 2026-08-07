using System.Collections.Concurrent;
using System.Reflection;
using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Application.Reporting;

public static class ExportFieldCatalog
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<ExportField>> Cache = new();

    public static IReadOnlyList<ExportField> For<T>() => Cache.GetOrAdd(typeof(T), Describe);

    private static IReadOnlyList<ExportField> Describe(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => (property, column: property.GetCustomAttribute<ExportColumnAttribute>()))
            .Where(entry => entry.column is not null && entry.property.CanRead)
            .OrderBy(entry => entry.column!.Order)
            .Select(entry => new ExportField(
                entry.property.Name,
                entry.column!.Header,
                entry.column.Format,
                entry.property.GetValue))
            .ToList();
}
