namespace DigitalWallet.Application.DTOs.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool KYCVerified { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
