using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountIsLiveRule : IPaymentRule
    {
        public bool CanMakePayment(Account account, MakePaymentRequest request)
            => account is {Status: AccountStatus.Live};
    }
}