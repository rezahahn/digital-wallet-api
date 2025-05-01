namespace DigitalWallet.Application.DTOs.Messaging
{
    public class PaymentNotificationMessage
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string FailureReason { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
