namespace DigitalWallet.Core.Exceptions
{
    public class PhoneNumberAlreadyExistsException : DigitalWalletException
    {
        public PhoneNumberAlreadyExistsException(string phoneNumber)
            : base($"Phone number '{phoneNumber}' is already registered")
        {
        }
    }
}
