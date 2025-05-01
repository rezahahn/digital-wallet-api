using RabbitMQ.Client;

namespace DigitalWallet.Core.Interfaces.Messaging
{
    public interface IRabbitMQConnection : IDisposable
    {
        IModel CreateChannel();
        bool IsConnected { get; }
        bool TryConnect();
    }
}
