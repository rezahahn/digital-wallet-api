namespace DigitalWallet.Core.Exceptions
{
    public class WalletOperationException : DigitalWalletException
    {
        public WalletOperationException(string message) : base(message)
        {
        }

        public WalletOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
