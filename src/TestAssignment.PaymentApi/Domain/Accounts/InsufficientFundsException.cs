namespace TestAssignment.PaymentApi.Domain.Accounts;

public sealed class InsufficientFundsException : Exception
{
    public InsufficientFundsException()
        : base("Insufficient funds.")
    {
    }
}