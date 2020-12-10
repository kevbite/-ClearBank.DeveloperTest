using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentSchemeRule : IPaymentRule
    {
        private readonly AllowedPaymentSchemes _paymentScheme;

        public PaymentSchemeRule(AllowedPaymentSchemes paymentScheme)
        {
            _paymentScheme = paymentScheme;
        }

        public bool CanMakePayment(Account account, MakePaymentRequest request)
            => account.AllowedPaymentSchemes.HasFlag(_paymentScheme);
    }
}