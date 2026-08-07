namespace ProductChallenge.Application.Messaging;

public interface IServiceBus<T>
{
    Task PublishAsync(T message);

    Task SubscribeAsync(Func<T, Task> handler);
}
