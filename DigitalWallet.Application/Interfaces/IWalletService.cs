using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.DTOs.Wallet.Request;
using DigitalWallet.Application.DTOs.Wallet.Response;

namespace DigitalWallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto?> GetWalletByUserIdAsync(int userId);
        Task<decimal> GetWalletBalanceAsync(int userId);
        Task<TopUpResponse> TopUpAsync(TopUpRequest request);
        Task<TransferResponse> TransferAsync(TransferRequest request);
        Task<bool> CreateWalletForUser(int userId);
        Task<WalletTransactionsResponse> GetWalletTransactionsAsync(
            int walletId, DateTime? fromDate, DateTime? toDate, string? transactionType, string? status, string sortBy
            , bool descending, int page, int pageSize);
    }
}
