namespace DigitalWallet.Core.Exceptions
{
    public class InvalidTransactionException : DigitalWalletException
    {
        public InvalidTransactionException(string message)
            : base(message)
        {
        }
    }
}
