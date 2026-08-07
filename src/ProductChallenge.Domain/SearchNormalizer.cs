using System.Globalization;
using System.Text;

namespace ProductChallenge.Domain;

public static class SearchNormalizer
{
    /// <summary>
    /// Precisa ser aplicado tanto ao texto gravado quanto ao termo buscado: os dois lados da
    /// comparação têm de estar na mesma forma.
    /// </summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();
    }
}
