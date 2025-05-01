namespace DigitalWallet.Application.DTOs.Wallet.Response
{
    public class TopUpResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public decimal NewBalance { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
