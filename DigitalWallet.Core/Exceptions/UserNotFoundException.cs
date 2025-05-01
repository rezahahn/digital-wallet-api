namespace DigitalWallet.Core.Exceptions
{
    public class UserNotFoundException : DigitalWalletException
    {
        public UserNotFoundException(int userId)
            : base($"User with ID {userId} not found")
        {
        }

        public UserNotFoundException(string email)
            : base($"User with email {email} not found")
        {
        }
    }
}
