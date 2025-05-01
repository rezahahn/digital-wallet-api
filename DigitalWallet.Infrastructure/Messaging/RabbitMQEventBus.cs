using DigitalWallet.Core.Interfaces.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace DigitalWallet.Infrastructure.Messaging
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private const int MaxRetryCount = 3;
        private const string RetryHeader = "x-retry-count";
        private const string WorkQueue = "create_wallet_queue.work";
        private const string DeadLetterQueue = "create_wallet_queue.dead";

        private readonly IRabbitMQConnection _connection;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IModel _consumerChannel;

        public RabbitMQEventBus(
            IRabbitMQConnection connection,
            ILogger<RabbitMQEventBus> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _connection = connection;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _consumerChannel = CreateConsumerChannel();
            InitializeQueues();
        }

        private void InitializeQueues()
        {
            // Deklarasi dead letter exchange
            _consumerChannel.ExchangeDeclare(
                exchange: "dead_letter_exchange",
                type: ExchangeType.Fanout,
                durable: true);

            // Deklarasi dead letter queue
            _consumerChannel.QueueDeclare(
                queue: DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _consumerChannel.QueueBind(
                queue: DeadLetterQueue,
                exchange: "dead_letter_exchange",
                routingKey: "");

            // Deklarasi work queue dengan DLX
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "dead_letter_exchange" }
            };

            _consumerChannel.QueueDeclare(
                queue: WorkQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);
        }

        public void Publish(string queueName, object message)
        {
            using var channel = _connection.CreateChannel();

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                { RetryHeader, 0 } // Inisialisasi retry count
            };

            channel.BasicPublish(
                exchange: "",
                routingKey: WorkQueue,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Mengirim pesan ke {QueueName}", WorkQueue);
        }

        public void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);

            consumer.Received += async (model, ea) =>
            {
                // Buat scope baru untuk setiap pesan
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var message = JsonSerializer.Deserialize<T>(ea.Body.Span);
                        if (message != null)
                        {
                            await handler(message);
                            _consumerChannel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saat memproses pesan");
                        HandleFailedMessage(ea);
                    }
                }
            };

            _consumerChannel.BasicConsume(
                queue: WorkQueue,
                autoAck: false,
                consumer: consumer);
        }

        private void HandleFailedMessage(BasicDeliverEventArgs ea)
        {
            var retryCount = GetRetryCount(ea.BasicProperties);

            if (retryCount < MaxRetryCount)
            {
                // Kirim ulang dengan retry count yang ditambah
                var properties = _consumerChannel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>
                {
                    { RetryHeader, retryCount + 1 }
                };

                _consumerChannel.BasicPublish(
                    exchange: "",
                    routingKey: WorkQueue,
                    basicProperties: properties,
                    body: ea.Body);

                _logger.LogWarning("Pesan dikirim ulang (retry ke {RetryCount})", retryCount + 1);
            }
            else
            {
                // Pindahkan ke dead letter queue
                _consumerChannel.BasicPublish(
                    exchange: "dead_letter_exchange",
                    routingKey: "",
                    basicProperties: null,
                    body: ea.Body);

                _logger.LogError("Pesan dipindahkan ke dead letter queue setelah {MaxRetryCount} retry", MaxRetryCount);
            }

            _consumerChannel.BasicAck(ea.DeliveryTag, false);
        }

        private static int GetRetryCount(IBasicProperties properties)
        {
            if (properties.Headers != null &&
                properties.Headers.TryGetValue(RetryHeader, out var value) &&
                value is int retryCount)
            {
                return retryCount;
            }
            return 0;
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            var channel = _connection.CreateChannel();
            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception, "Error callback channel RabbitMQ");
            };

            channel.ModelShutdown += (sender, ea) =>
            {
                _logger.LogWarning($"Channel RabbitMQ dimatikan: {ea.ReplyText}");
            };

            return channel;
        }

        public void Dispose()
        {
            try
            {
                _consumerChannel?.Close();
                _logger.LogInformation("Channel RabbitMQ ditutup");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saat menutup channel RabbitMQ");
            }
        }
    }
}
