namespace DigitalWallet.Application.DTOs.Wallet.Request
{
    public class TopUpRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // BankTransfer, CreditCard, etc.
    }
}
