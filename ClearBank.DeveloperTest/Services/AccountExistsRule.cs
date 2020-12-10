using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountExistsRule : IPaymentRule
    {
        public bool CanMakePayment(Account account, MakePaymentRequest request)
            => account is not null;
    }
}