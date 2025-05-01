namespace DigitalWallet.Core.Exceptions
{
    public abstract class DigitalWalletException : Exception
    {
        public DigitalWalletException(string? message) : base(message)
        {
        }

        protected DigitalWalletException(string message, Exception innerException) : base(message)
        {
        }
    }
}
