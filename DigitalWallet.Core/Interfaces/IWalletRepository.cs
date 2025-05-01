using DigitalWallet.Core.Entities;

namespace DigitalWallet.Core.Interfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(int userId);
        Task<bool> UserHasWalletAsync(int userId);
        Task<bool> WalletExistsAsync(int walletId);
        Task<(IEnumerable<Transaction> transactions, int totalCount)> GetTransactionsByWalletIdAsync(
        int walletId,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize);
    }
}
