using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.MakingPayments
{
    public class BacsPaymentTests : IAccountDataStoreFactory, IAccountDataStore
    {
        private readonly PaymentService _service;
        private readonly Dictionary<string, Account> _accounts = new();
        private Account _updatedAccount;

        public BacsPaymentTests()
        {
            _service = new PaymentService(this);
        }

        [Fact]
        public void ShouldReturnSuccessfulAndUpdateAccountBalance()
        {
            var balance = 150;
            var preAccount = CreateAccount(balance);

            var makePaymentRequest = CreateMakeBacsPaymentRequest()
                with{
                DebtorAccountNumber = preAccount.AccountNumber,
                Amount = 50
                };
            var result = _service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result.Should().BeEquivalentTo(new
            {
                Success = true
            });
            _updatedAccount.Should().BeEquivalentTo(preAccount,
                opt => opt.Excluding(x => x.Balance));
            _updatedAccount.Balance.Should().Be(100);
        }

        private Account CreateAccount(int balance = 100)
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

        [Fact]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountThatDoesNotExist()
        {
            var makePaymentRequest = CreateMakeBacsPaymentRequest();

            _service.MakePayment(makePaymentRequest)
                .Should().BeEquivalentTo(new
                {
                    Success = false
                });
        }

        [Fact]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountsThatDontAllowBacsPaymentSchemes()
        {
            var account = CreateAccount();
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.All ^ AllowedPaymentSchemes.Bacs;

            var makePaymentRequest = CreateMakeBacsPaymentRequest()
                with {
                    DebtorAccountNumber = account.AccountNumber
                };

            _service.MakePayment(makePaymentRequest)
                .Should().BeEquivalentTo(new
                {
                    Success = false
                });
        }

        private static MakePaymentRequest CreateMakeBacsPaymentRequest()
        {
            var makePaymentRequest = new MakePaymentRequest(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                10,
                DateTime.UtcNow,
                PaymentScheme.Bacs
            );
            return makePaymentRequest;
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
            _updatedAccount = account;
        }
    }
}