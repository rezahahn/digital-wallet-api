namespace DigitalWallet.Core.Exceptions
{
    public class InsufficientBalanceException : DigitalWalletException
    {
        public InsufficientBalanceException()
            : base("Insufficient balance for this transaction")
        {
        }
    }
}
