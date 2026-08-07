using System.Collections.Concurrent;
using System.Reflection;
using ProductChallenge.Domain.Validation.Rules;

namespace ProductChallenge.Domain.Validation;

public sealed record ValidatedProperty(string Name, Attribute[] Attributes, Func<object, object?> Read);

public class DynamicValidator
{
    private static readonly IValidationRule[] Rules =
    [
        new RequiredRule(),
        new TextLengthRule(),
        new NumberRangeRule()
    ];

    // Resolver propriedades e atributos por reflexão a cada chamada custaria caro numa tela que
    // valida a cada tentativa de salvar; o mapa por tipo é montado uma vez.
    private static readonly ConcurrentDictionary<Type, ValidatedProperty[]> MetadataCache = new();

    public static ValidationResult Validate(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var failures = new List<ValidationFailure>();

        foreach (var property in MetadataCache.GetOrAdd(obj.GetType(), Describe))
        {
            var value = property.Read(obj);

            foreach (var attribute in property.Attributes)
            {
                var rule = Array.Find(Rules, candidate => candidate.AttributeType.IsInstanceOfType(attribute));

                if (rule?.Check(attribute, property, value) is { } failure)
                {
                    // Uma mensagem por campo: acumular "obrigatório" e "fora da faixa" no mesmo
                    // controle só polui a tela.
                    failures.Add(failure);
                    break;
                }
            }
        }

        return ValidationResult.Failed(failures);
    }

    private static ValidatedProperty[] Describe(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead)
            .Select(property => new ValidatedProperty(
                property.Name,
                property.GetCustomAttributes().ToArray(),
                property.GetValue))
            .Where(property => property.Attributes.Length > 0)
            .ToArray();
}
