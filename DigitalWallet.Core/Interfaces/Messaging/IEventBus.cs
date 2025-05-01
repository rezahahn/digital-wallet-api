namespace DigitalWallet.Core.Interfaces.Messaging
{
    public interface IEventBus : IDisposable
    {
        void Publish(string queueName, object message);
        void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class;
    }
}
