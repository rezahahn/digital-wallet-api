namespace DigitalWallet.Core.Entities
{
    public class UserInvestment
    {
        public int InvestmentId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal? CurrentValue { get; set; }
        public DateTime InvestmentDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public string Status { get; set; } = "Active";
        public int? TransactionId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public InvestmentProduct Product { get; set; }
        public Transaction Transaction { get; set; }
    }
}
