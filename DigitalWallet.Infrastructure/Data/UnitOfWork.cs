using DigitalWallet.Core.Exceptions;
using DigitalWallet.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DigitalWalletContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed;

        public UnitOfWork(
            DigitalWalletContext context,
            ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
                return;

            _logger.LogInformation("Beginning transaction");
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_context.Database.CurrentTransaction != null)
                {
                    _logger.LogInformation("Committing transaction");
                    await _context.Database.CommitTransactionAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_context.Database.CurrentTransaction != null)
                {
                    _logger.LogWarning("Rolling back transaction");
                    await _context.Database.RollbackTransactionAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during rollback");
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                _logger.LogInformation("Saving changes");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                throw new DatabaseOperationException("Failed to save changes to database", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed && _context != null)
            {
                _disposed = true;
                _context.Dispose();
            }
        }
    }
}
