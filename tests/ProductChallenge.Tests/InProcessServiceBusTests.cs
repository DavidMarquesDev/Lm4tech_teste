using Microsoft.Extensions.Logging.Abstractions;
using ProductChallenge.Application.Messaging;
using ProductChallenge.Infrastructure.Messaging;

namespace ProductChallenge.Tests;

public class InProcessServiceBusTests
{
    private readonly InProcessServiceBus<string> _bus =
        new(NullLogger<InProcessServiceBus<string>>.Instance);

    [Fact]
    public async Task SubscribeAsync_WithNullHandler_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bus.SubscribeAsync(null!));
    }

    [Fact]
    public async Task PublishAsync_WithoutSubscribers_DoesNothing()
    {
        await _bus.PublishAsync("mensagem");
    }

    [Fact]
    public async Task PublishAsync_DeliversToEverySubscriber()
    {
        var received = new List<string>();

        await _bus.SubscribeAsync(message => { received.Add($"a:{message}"); return Task.CompletedTask; });
        await _bus.SubscribeAsync(message => { received.Add($"b:{message}"); return Task.CompletedTask; });

        await _bus.PublishAsync("oi");

        Assert.Equal(2, received.Count);
        Assert.Contains("a:oi", received);
        Assert.Contains("b:oi", received);
    }

    [Fact]
    public async Task PublishAsync_AFailingSubscriberDoesNotBlockTheOthers()
    {
        var delivered = false;

        await _bus.SubscribeAsync(_ => throw new InvalidOperationException("assinante com defeito"));
        await _bus.SubscribeAsync(_ => { delivered = true; return Task.CompletedTask; });

        await _bus.PublishAsync("oi");

        Assert.True(delivered);
    }

    [Fact]
    public async Task PublishAsync_DoesNotPropagateSubscriberFailures()
    {
        await _bus.SubscribeAsync(_ => throw new InvalidOperationException("assinante com defeito"));

        await _bus.PublishAsync("oi");
    }

    [Fact]
    public async Task SubscribeAsync_DuringPublish_DoesNotBreakTheIteration()
    {
        var received = 0;

        await _bus.SubscribeAsync(async _ =>
        {
            received++;
            await _bus.SubscribeAsync(__ => Task.CompletedTask);
        });

        await _bus.PublishAsync("oi");

        Assert.Equal(1, received);
    }

    [Fact]
    public async Task PublishAsync_SubscribersReceiveTheSameMessage()
    {
        ProductChangedNotification? received = null;
        var bus = new InProcessServiceBus<ProductChangedNotification>(
            NullLogger<InProcessServiceBus<ProductChangedNotification>>.Instance);

        await bus.SubscribeAsync(notification => { received = notification; return Task.CompletedTask; });
        await bus.PublishAsync(new ProductChangedNotification(7, "Monitor", ProductChange.Updated, []));

        Assert.Equal(new ProductChangedNotification(7, "Monitor", ProductChange.Updated, []), received);
    }
}
