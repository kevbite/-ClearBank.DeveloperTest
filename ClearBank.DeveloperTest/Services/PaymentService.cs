using ClearBank.DeveloperTest.Types;
using System.Linq;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory _dataStoreFactory;

        public PaymentService(IAccountDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStore = _dataStoreFactory.Create();
            var account = dataStore.GetAccount(request.DebtorAccountNumber);

            if (CanMakePayment(request, account))
            {
                account.Balance -= request.Amount;
                dataStore.UpdateAccount(account);

                return MakePaymentResult.CreateSuccess();
            }

            return MakePaymentResult.CreateFail();
        }

        private static bool CanMakePayment(MakePaymentRequest request, Account account)
        {
            var rules = new PaymentRuleFactory()
                .GetRules(request.PaymentScheme);

            var canMakePayment = rules.All(x => x.CanMakePayment(account, request));
            return canMakePayment;
        }
    }
}