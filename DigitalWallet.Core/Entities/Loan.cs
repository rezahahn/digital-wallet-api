namespace DigitalWallet.Core.Entities
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int TermMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Active";
        public string Purpose { get; set; } = string.Empty;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User Approver { get; set; }
        public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();
    }
}
