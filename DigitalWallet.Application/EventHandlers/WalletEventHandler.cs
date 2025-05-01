using DigitalWallet.Application.DTOs.Messaging;
using DigitalWallet.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DigitalWallet.Application.EventHandlers
{
    public class WalletEventHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WalletEventHandler> _logger;

        public WalletEventHandler(
            IServiceProvider serviceProvider,
            ILogger<WalletEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task HandleCreateWalletMessage(CreateWalletMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            try
            {
                _logger.LogInformation("Processing wallet creation for user: {UserId}", message.UserId);
                var success = await walletService.CreateWalletForUser(message.UserId);

                if (!success)
                {
                    throw new Exception("Failed to create wallet (service returned false)");
                }

                _logger.LogInformation("Successfully created wallet for user: {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create wallet for user {UserId}", message.UserId);
                throw;
            }
        }
    }
}
