namespace TestAssignment.IdentityApi.Domain.Sesssions;


public readonly record struct SessionId(Guid Value)
{
    public static SessionId New()
    {
        return new(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}