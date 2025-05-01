using DigitalWallet.Core.Entities;

namespace DigitalWallet.Core.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
        Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber);
        Task<(IEnumerable<Transaction> transactions, int totalCount)> GetTransactionsByWalletIdAsync(
                int walletId,
                DateTime? fromDate,
                DateTime? toDate,
                string? transactionType,
                string? status,
                string sortBy,
                bool descending,
                int page,
                int pageSize);
    }
}
