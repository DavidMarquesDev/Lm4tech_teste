using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductChallenge.Application.Messaging;

namespace ProductChallenge.Infrastructure.Messaging;

/// <summary>
/// Segundo assinante do mesmo evento. É ele que comprova o desacoplamento: registra auditoria
/// sem que o serviço que grava saiba da sua existência.
/// </summary>
public static class ProductAuditLog
{
    public static void SubscribeAuditLog(this IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var bus = provider.GetRequiredService<IServiceBus<ProductChangedNotification>>();
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("Auditoria");

        // A assinatura do enunciado é assíncrona, mas num bus em memória ela conclui de imediato.
        bus.SubscribeAsync(notification =>
        {
            logger.LogInformation(
                "Produto {ProductId} \"{ProductName}\" {Action}{Detail}",
                notification.ProductId,
                notification.ProductName,
                Action(notification.Change),
                Detail(notification.Changes));

            return Task.CompletedTask;
        }).GetAwaiter().GetResult();
    }

    private static string Action(ProductChange change) => change switch
    {
        ProductChange.Created => "criado",
        ProductChange.Updated => "atualizado",
        ProductChange.Deleted => "excluído",
        _ => change.ToString()
    };

    private static string Detail(IReadOnlyList<FieldChange> changes) =>
        changes.Count == 0 ? string.Empty : " — " + string.Join("; ", changes.Select(Describe));

    private static string Describe(FieldChange change) => change.Before is null
        ? $"{change.Field}: {change.After}"
        : $"{change.Field}: {change.Before} → {change.After}";
}
