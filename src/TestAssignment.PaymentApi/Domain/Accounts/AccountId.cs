namespace TestAssignment.PaymentApi.Domain.Accounts;

public readonly record struct AccountId(Guid Value)
{
    public static AccountId New()
    {
        return new(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}