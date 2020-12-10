namespace ClearBank.DeveloperTest.Types
{
    public record MakePaymentResult(bool Success)
    {
        public static MakePaymentResult CreateSuccess()
        {
            return new(true);
        }
        public static MakePaymentResult CreateFail()
        {
            return new(false);
        }
    }
}