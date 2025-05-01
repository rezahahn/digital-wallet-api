namespace DigitalWallet.Core.Entities
{
    public class LoanPayment
    {
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public int? TransactionId { get; set; }

        // Navigation properties
        public Loan Loan { get; set; }
        public Transaction Transaction { get; set; }
    }
}
