using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Tests.MakingPayments
{
    public class PaymentsHarness : IAccountDataStoreFactory, IAccountDataStore
    {
        public PaymentService Service { get; }
        private readonly Dictionary<string, Account> _accounts = new();
        private readonly List<Account> _updatedAccounts = new();
        
        public IReadOnlyCollection<Account> UpdatedAccounts => _updatedAccounts;

        public PaymentsHarness()
        {
            Service = new PaymentService(this);
        }
        public Account CreateAccount(decimal balance = 100M)
        {
            var account = new Account()
            {
                Balance = balance,
                Status = AccountStatus.Live,
                AccountNumber = Guid.NewGuid().ToString(),
                AllowedPaymentSchemes = AllowedPaymentSchemes.All
            };

            _accounts.Add(account.AccountNumber, account);

            return account;
        }
        IAccountDataStore IAccountDataStoreFactory.Create()
        {
            return this;
        }

        Account IAccountDataStore.GetAccount(string accountNumber)
        {
            return _accounts.TryGetValue(accountNumber, out var account)
                is true
                ? account
                : null;
        }

        void IAccountDataStore.UpdateAccount(Account account)
        {
            _updatedAccounts.Add(account);
        }
    }
}