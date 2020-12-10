using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public interface IAccountDataStoreFactory
    {
        IAccountDataStore Create();
    }
    public class AccountDataStoreFactory : IAccountDataStoreFactory
    {
        private readonly DataStoreOptions _options;

        public AccountDataStoreFactory(DataStoreOptions options)
        {
            _options = options;
        }

        public IAccountDataStore Create()
        {
            return _options.DataStoreType switch
            {
                "Backup" => new BackupAccountDataStore(),
                _ => new AccountDataStore()
            };
        }
    }

    public class DataStoreOptions
    {
        public string DataStoreType => 
           ConfigurationManager.AppSettings["DataStoreType"];
    }

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

            var result = new MakePaymentResult(){Success = true};

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }

                    break;

                case PaymentScheme.FasterPayments:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        result.Success = false;
                    }
                    else if (account.Balance < request.Amount)
                    {
                        result.Success = false;
                    }

                    break;

                case PaymentScheme.Chaps:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        result.Success = false;
                    }
                    else if (account.Status != AccountStatus.Live)
                    {
                        result.Success = false;
                    }

                    break;
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;
                dataStore.UpdateAccount(account);
            }

            return result;
        }
    }
}