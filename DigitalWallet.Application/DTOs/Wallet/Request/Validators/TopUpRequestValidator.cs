using FluentValidation;

namespace DigitalWallet.Application.DTOs.Wallet.Request.Validators
{
    public class TopUpRequestValidator : AbstractValidator<TopUpRequest>
    {
        public TopUpRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be positive");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0")
                .LessThan(1000000000).WithMessage("Amount is too large");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required")
                .Must(BeValidPaymentMethod).WithMessage("Invalid payment method");
        }

        private bool BeValidPaymentMethod(string method)
        {
            var validMethods = new[] { "BankTransfer", "CreditCard", "E-Wallet" };
            return validMethods.Contains(method);
        }
    }
}
