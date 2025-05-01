namespace DigitalWallet.Core.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; // TopUp, Transfer, etc.
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "Completed";
        public string ReferenceNumber { get; set; } = string.Empty;
        public int? RelatedTransactionId { get; set; }

        // Navigation properties
        public Wallet Wallet { get; set; }
        public Transaction RelatedTransaction { get; set; }
        public LoanPayment LoanPayment { get; set; }
        public UserInvestment Investment { get; set; }
    }
}
