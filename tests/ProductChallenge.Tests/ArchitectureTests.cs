using System.Reflection;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Desktop.ViewModels;
using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Repositories;

namespace ProductChallenge.Tests;

/// <summary>
/// Transforma a regra de dependência em algo verificável: sem estes testes ela seria apenas
/// um acordo que ninguém confere quando alguém adiciona um <c>using</c> conveniente.
/// </summary>
public class ArchitectureTests
{
    private const string EntityFramework = "Microsoft.EntityFrameworkCore";
    private const string WindowsForms = "System.Windows.Forms";

    private static IReadOnlyList<string> ReferencesOf<T>() =>
        typeof(T).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name ?? string.Empty)
            .ToList();

    private static void AssertDoesNotReference<T>(params string[] forbiddenPrefixes)
    {
        var references = ReferencesOf<T>();

        var violations = references
            .Where(reference => forbiddenPrefixes.Any(
                prefix => reference.StartsWith(prefix, StringComparison.Ordinal)))
            .ToList();

        Assert.True(
            violations.Count == 0,
            $"{typeof(T).Assembly.GetName().Name} não deveria referenciar: {string.Join(", ", violations)}");
    }

    [Fact]
    public void Domain_DependsOnNothingButTheFramework()
    {
        AssertDoesNotReference<Product>(
            EntityFramework, WindowsForms, "ProductChallenge.", "CommunityToolkit");
    }

    [Fact]
    public void Application_DoesNotKnowHowDataIsStored()
    {
        AssertDoesNotReference<IProductService>(
            EntityFramework, WindowsForms, "ProductChallenge.Infrastructure", "ProductChallenge.Desktop");
    }

    [Fact]
    public void Presentation_DoesNotTouchEntityFramework()
    {
        AssertDoesNotReference<ProductListViewModel>(EntityFramework);
    }

    [Fact]
    public void Infrastructure_DoesNotDependOnPresentation()
    {
        AssertDoesNotReference<ProductRepository>(WindowsForms, "ProductChallenge.Desktop");
    }

    [Fact]
    public void ViewModels_DoNotReferenceWindowsFormsTypes()
    {
        var offenders = typeof(ProductListViewModel).Assembly
            .GetTypes()
            .Where(type => type.Namespace?.StartsWith("ProductChallenge.Desktop.ViewModels", StringComparison.Ordinal) == true)
            .SelectMany(type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(property => property.PropertyType.Namespace?.StartsWith(WindowsForms, StringComparison.Ordinal) == true)
            .Select(property => $"{property.DeclaringType?.Name}.{property.Name}")
            .ToList();

        Assert.True(offenders.Count == 0, $"Propriedades expondo tipos de UI: {string.Join(", ", offenders)}");
    }
}
