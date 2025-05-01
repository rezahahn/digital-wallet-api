namespace DigitalWallet.Application.DTOs.Wallet.Response
{
    public class WalletTransactionsResponse
    {
        public IEnumerable<WalletTransactionDto> Transactions { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public DateTime? FilterToDate { get; set; }
        public string? FilterType { get; set; }
        public string? FilterStatus { get; set; }
    }
}
