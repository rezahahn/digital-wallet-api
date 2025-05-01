namespace DigitalWallet.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public bool KYCVerified { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public Wallet Wallet { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<UserInvestment> Investments { get; set; } = new List<UserInvestment>();
    }
}
