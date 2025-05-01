namespace DigitalWallet.Core.Exceptions
{
    public class WalletNotFoundException : DigitalWalletException
    {
        public WalletNotFoundException(int walletId)
            : base($"Wallet with ID {walletId} not found")
        {
        }

        public WalletNotFoundException(string message)
            : base(message)
        {
        }
    }
}
