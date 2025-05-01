namespace DigitalWallet.Core.Exceptions
{
    public class EmailAlreadyExistsException : DigitalWalletException
    {
        public EmailAlreadyExistsException(string email)
            : base($"Email '{email}' is already registered")
        {
        }
    }
}
