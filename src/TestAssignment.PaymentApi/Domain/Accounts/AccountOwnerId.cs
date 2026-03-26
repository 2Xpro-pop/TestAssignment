namespace TestAssignment.PaymentApi.Domain.Accounts;

public readonly record struct AccountOwnerId(Guid Value)
{
    public static AccountOwnerId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Owner id cannot be empty.", nameof(value));
        }

        return new(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}