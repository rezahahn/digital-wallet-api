using DigitalWallet.Core.Interfaces.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DigitalWallet.Infrastructure.Messaging
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(
            IConfiguration configuration,
            ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connectionFactory = factory; // assign as IConnectionFactory

            _logger.LogInformation($"Initializing RabbitMQ connection to {factory.HostName}");
        }

        public IModel CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available");

            return _connection.CreateModel();
        }

        public bool IsConnected => _connection?.IsOpen == true && !_disposed;

        public bool TryConnect()
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ...");
                _connection = _connectionFactory.CreateConnection();
                _logger.LogInformation("Successfully connected to RabbitMQ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ");
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            try
            {
                _connection?.Close();
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing RabbitMQ connection");
            }
        }
    }
}
