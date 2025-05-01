namespace DigitalWallet.Infrastructure.Messaging
{
    public class RabbitMQConfig
    {
        public required string HostName { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string CreateWalletQueue { get; set; } = "create_wallet_queue";
    }
}
