using System;

namespace ClearBank.DeveloperTest.Types
{
    public record MakePaymentRequest(
        string CreditorAccountNumber,
        string DebtorAccountNumber,
        decimal Amount,
        DateTime PaymentDate,
        PaymentScheme PaymentScheme
    );
}
