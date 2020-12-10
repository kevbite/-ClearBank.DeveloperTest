using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public interface IPaymentRule
    {
        bool CanMakePayment(Account account, MakePaymentRequest request);
    }
}