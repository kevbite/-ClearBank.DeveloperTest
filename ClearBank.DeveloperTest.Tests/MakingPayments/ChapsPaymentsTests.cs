using System;
using System.Linq;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.MakingPayments
{
    public class ChapsPaymentsTests
    {
        private readonly PaymentsHarness _harness;

        public ChapsPaymentsTests()
        {
            _harness = new PaymentsHarness();
        }

        [Fact]
        public void ShouldReturnSuccessfulAndUpdateAccountBalance()
        {
            var preAccount = _harness.CreateAccount(100);

            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest()
                with{
                DebtorAccountNumber = preAccount.AccountNumber,
                Amount = 25
                };
            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result.Should().BeEquivalentTo(new
            {
                Success = true
            });
            _harness.UpdatedAccounts.Single().Should().BeEquivalentTo(preAccount,
                opt => opt.Excluding(x => x.Balance));
            _harness.UpdatedAccounts.Single().Balance.Should().Be(75);
        }

        [Fact]
        public void ShouldReturnUnSuccessfulAndNotUpdateAccountsWhenNoAccountExits()
        {
            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest();
            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result.Should().BeEquivalentTo(new
            {
                Success = false
            });
            _harness.UpdatedAccounts.Should().BeEmpty();
        }

        [Fact]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountsThatDontAllowChapsPaymentsSchemes()
        {
            var account = _harness.CreateAccount();
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.All ^ AllowedPaymentSchemes.Chaps;

            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest()
                with {
                DebtorAccountNumber = account.AccountNumber
                };

            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result
                .Should().BeEquivalentTo(new
                {
                    Success = false
                });
            _harness.UpdatedAccounts.Should().BeEmpty();
        }
        
        
        [Theory]
        [InlineData(AccountStatus.Disabled)]
        [InlineData(AccountStatus.InboundPaymentsOnly)]
        public void ShouldReturnUnSuccessfulWhenAccountStatusIsNotLive(AccountStatus accountStatus)
        {
            var account = _harness.CreateAccount();
            account.Status = accountStatus;

            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest()
                with {
                DebtorAccountNumber = account.AccountNumber
                };

            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result
                .Should().BeEquivalentTo(new
                {
                    Success = false
                });
            _harness.UpdatedAccounts.Should().BeEmpty();
        }

        private static MakePaymentRequest CreateMakeFasterPaymentsPaymentRequest()
        {
            var makePaymentRequest = new MakePaymentRequest(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                10,
                DateTime.UtcNow,
                PaymentScheme.Chaps
            );
            return makePaymentRequest;
        }
    }
}