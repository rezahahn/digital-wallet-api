namespace DigitalWallet.Application.DTOs.User
{
    public class VerifyKycDto
    {
        public int UserId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
    }
}
