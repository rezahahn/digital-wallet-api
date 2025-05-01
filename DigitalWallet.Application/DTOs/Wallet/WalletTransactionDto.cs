namespace DigitalWallet.Application.DTOs.Wallet
{
    public class WalletTransactionDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
