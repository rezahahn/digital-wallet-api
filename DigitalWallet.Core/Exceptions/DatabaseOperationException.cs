namespace DigitalWallet.Core.Exceptions
{
    public class DatabaseOperationException : DigitalWalletException
    {
        public DatabaseOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
