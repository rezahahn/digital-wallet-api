using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories
{
    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(DigitalWalletContext context) : base(context)
        {
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<bool> UserHasWalletAsync(int userId)
        {
            return await _context.Wallets
                .AnyAsync(w => w.UserId == userId);
        }

        public async Task<bool> WalletExistsAsync(int walletId)
        {
            return await _context.Wallets
                .AnyAsync(w => w.WalletId == walletId);
        }

        public async Task<(IEnumerable<Transaction> transactions, int totalCount)>
        GetTransactionsByWalletIdAsync(
            int walletId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            var query = _context.Transactions
                .Where(t => t.WalletId == walletId);

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value);

            var totalCount = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }
    }
}
