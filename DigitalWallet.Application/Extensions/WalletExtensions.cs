using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Core.Entities;

namespace DigitalWallet.Application.Extensions;

public static class WalletExtensions
{
    public static WalletDto ToWalletDto(this Wallet wallet)
    {
        return new WalletDto
        {
            WalletId = wallet.WalletId,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            LastUpdated = wallet.LastUpdated
        };
    }
}