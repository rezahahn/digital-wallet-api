using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(DigitalWalletContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
        {
            return await _context.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByActionTypeAsync(string actionType)
        {
            return await _context.AuditLogs
                .Where(log => log.ActionType == actionType)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}
