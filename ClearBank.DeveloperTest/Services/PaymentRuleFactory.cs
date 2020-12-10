using System;
using System.Collections.Generic;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentRuleFactory
    {
        public IEnumerable<IPaymentRule> GetRules(PaymentScheme paymentScheme)
        {
            yield return new AccountExistsRule();

            yield return new PaymentSchemeRule(GetAllowedPaymentSchemes(paymentScheme));

            if (paymentScheme is PaymentScheme.FasterPayments)
            {
                yield return new AccountHasFundsRule();
            }
            
            if (paymentScheme is PaymentScheme.Chaps)
            {
                yield return new AccountIsLiveRule();
            }
        }

        private AllowedPaymentSchemes GetAllowedPaymentSchemes(PaymentScheme paymentScheme)
            => paymentScheme switch
            {
                PaymentScheme.Bacs => AllowedPaymentSchemes.Bacs,
                PaymentScheme.FasterPayments => AllowedPaymentSchemes.FasterPayments,
                PaymentScheme.Chaps => AllowedPaymentSchemes.Chaps,
                _ => throw new ArgumentOutOfRangeException(nameof(paymentScheme), paymentScheme, "Unknown payment scheme.")
            };
    }
}