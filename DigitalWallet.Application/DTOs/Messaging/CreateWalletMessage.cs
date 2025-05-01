namespace DigitalWallet.Application.DTOs.Messaging
{
    public class CreateWalletMessage
    {
        public int UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow.ToLocalTime();
    }
}
