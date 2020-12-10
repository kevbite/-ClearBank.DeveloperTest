using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class AccountHasFundsRule : IPaymentRule
    {
        public bool CanMakePayment(Account account, MakePaymentRequest request)
            => account.Balance >= request.Amount;
    }
}