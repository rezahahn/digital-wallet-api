namespace DigitalWallet.Application.DTOs.Wallet.Response
{
    public class TransferResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public decimal SenderNewBalance { get; set; }
        public decimal ReceiverNewBalance { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
