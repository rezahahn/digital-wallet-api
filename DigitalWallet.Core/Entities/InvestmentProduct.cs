namespace DigitalWallet.Core.Entities
{
    public class InvestmentProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ExpectedReturnRate { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public decimal MinimumInvestment { get; set; }
        public int DurationMonths { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public ICollection<UserInvestment> UserInvestments { get; set; } = new List<UserInvestment>();
    }
}
