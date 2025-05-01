namespace DigitalWallet.Application.DTOs.Wallet.Request
{
    public class TransferRequest
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
