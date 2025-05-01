namespace DigitalWallet.Application.DTOs.Wallet
{
    public class WalletDto
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
