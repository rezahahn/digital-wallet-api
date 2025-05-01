using DigitalWallet.Core.Entities;

namespace DigitalWallet.Core.Interfaces
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AuditLog>> GetByActionTypeAsync(string actionType);
    }
}
