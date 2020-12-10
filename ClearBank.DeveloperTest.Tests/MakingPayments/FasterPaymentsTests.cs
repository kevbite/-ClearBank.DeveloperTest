using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.MakingPayments
{

    public class FasterPaymentsTests 
    {
        private readonly PaymentsHarness _harness;

        public FasterPaymentsTests()
        {
            _harness = new PaymentsHarness();
        }
        
        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 1)]
        [InlineData(100, 50)]
        public void ShouldReturnSuccessfulAndUpdateAccountBalance(decimal accountBalance, decimal payment)
        {
            var preAccount = _harness.CreateAccount(accountBalance);

            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest()
                with{
                DebtorAccountNumber = preAccount.AccountNumber,
                Amount = payment
                };
            var result = _harness.Service.MakePayment(makePaymentRequest);

            using var _ = new AssertionScope();
            result.Should().BeEquivalentTo(new
            {
                Success = true
            });
            _harness.UpdatedAccounts.Single().Should().BeEquivalentTo(preAccount,
                opt => opt.Excluding(x => x.Balance));
            _harness.UpdatedAccounts.Single().Balance.Should().Be(accountBalance - payment);
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
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountsThatDontAllowFasterPaymentsSchemes()
        {
            var account = _harness.CreateAccount();
            account.AllowedPaymentSchemes = AllowedPaymentSchemes.All ^ AllowedPaymentSchemes.FasterPayments;

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
        [InlineData(100, 101)]
        [InlineData(100, 102)]
        [InlineData(100, 103)]
        [InlineData(50, 100)]
        [InlineData(0, 1)]
        public void ShouldReturnUnSuccessfulWhenMakingPaymentForAccountsThatDontNotHaveEnoughFunds(decimal accountBalance, decimal payment)
        {
            var account = _harness.CreateAccount(accountBalance);

            var makePaymentRequest = CreateMakeFasterPaymentsPaymentRequest()
                with {
                DebtorAccountNumber = account.AccountNumber,
                Amount = payment
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
                PaymentScheme.FasterPayments
            );
            return makePaymentRequest;
        }
       
    }
}