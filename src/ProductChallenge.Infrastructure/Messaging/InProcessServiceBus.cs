using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using ProductChallenge.Application.Messaging;

namespace ProductChallenge.Infrastructure.Messaging;

public sealed class InProcessServiceBus<T> : IServiceBus<T>
{
    private readonly ConcurrentDictionary<Guid, Func<T, Task>> _handlers = new();
    private readonly ILogger<InProcessServiceBus<T>> _logger;

    public InProcessServiceBus(ILogger<InProcessServiceBus<T>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task SubscribeAsync(Func<T, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers[Guid.NewGuid()] = handler;
        _logger.LogDebug("Assinante registrado para {MessageType}.", typeof(T).Name);

        return Task.CompletedTask;
    }

    public async Task PublishAsync(T message)
    {
        // Snapshot: um assinante pode se registrar enquanto a publicação acontece.
        var handlers = _handlers.ToArray();

        if (handlers.Length == 0)
        {
            return;
        }

        var deliveries = handlers.Select(async entry =>
        {
            try
            {
                await entry.Value(message);
            }
            catch (Exception exception)
            {
                // Um assinante com defeito não pode impedir a entrega aos demais.
                _logger.LogError(
                    exception, "Assinante {HandlerId} falhou ao tratar {MessageType}.",
                    entry.Key, typeof(T).Name);
            }
        });

        await Task.WhenAll(deliveries);
    }
}
