using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DigitalWallet.Infrastructure.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DigitalWalletContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber);
        }

        public async Task<(IEnumerable<Transaction> transactions, int totalCount)> GetTransactionsByWalletIdAsync(
        int walletId,
        DateTime? fromDate,
        DateTime? toDate,
        string? transactionType,
        string? status,
        string sortBy,
        bool descending,
        int page,
        int pageSize)
        {
            var query = _context.Transactions
                .Where(t => t.WalletId == walletId);

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value);

            if (!string.IsNullOrEmpty(transactionType))
                query = query.Where(t => t.TransactionType == transactionType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            query = descending ?
                query.OrderByDescending(GetSortProperty(sortBy)) :
                query.OrderBy(GetSortProperty(sortBy));

            var totalCount = await query.CountAsync();
            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        private Expression<Func<Transaction, object>> GetSortProperty(string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "amount" => t => t.Amount,
                "type" => t => t.TransactionType,
                "status" => t => t.Status,
                _ => t => t.TransactionDate
            };
        }
    }
}
