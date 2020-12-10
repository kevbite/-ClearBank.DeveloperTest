using System;
using System.Linq;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.MakingPayments
{
    public class BacsPaymentTests
    {
        private readonly PaymentsHarness _harness;

        public BacsPaymentTests()
        {
            _harness = new PaymentsHarness();
        }

        [Fact]
        public void ShouldReturnSuccessfulAndUpdateAccountBalance()
        {
            var balance = 150;
            var preAccount = _harness.CreateAccount(balance);

            var makePaymentRequest = CreateMakeBacsPaymentRequest()
                with{
                DebtorAccountNumber = preAccount.AccountNumber,
                Amount = 50
                };
            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result.Should().BeEquivalentTo(new
            {
                Success = true
            });
            _harness.UpdatedAccounts.First().Should().BeEquivalentTo(preAccount,
                opt => opt.Excluding(x => x.Balance));
            _harness.UpdatedAccounts.First().Balance.Should().Be(100);
        }



        [Fact]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountThatDoesNotExist()
        {
            var makePaymentRequest = CreateMakeBacsPaymentRequest();

            var makePaymentResult = _harness.Service.MakePayment(makePaymentRequest);
            
            using var _ = new AssertionScope();
            makePaymentResult
                .Should().BeEquivalentTo(new
                {
                    Success = false
                });
            _harness.UpdatedAccounts.Should().BeEmpty();

        }

        [Fact]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountsThatDontAllowBacsPaymentSchemes()
        {
            var account = _harness.CreateAccount();
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.All ^ AllowedPaymentSchemes.Bacs;

            var makePaymentRequest = CreateMakeBacsPaymentRequest()
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

    }
}