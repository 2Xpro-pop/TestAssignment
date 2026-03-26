namespace TestAssignment.PaymentApi.Domain.Payments;

public readonly record struct PaymentId(Guid Value)
{
    public static PaymentId New()
    {
        return new(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}