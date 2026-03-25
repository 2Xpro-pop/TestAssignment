namespace TestAssignment.IdentityApi.Domain.Sesssions;

public readonly record struct TokenHash
{
    public TokenHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Token hash cannot be null or whitespace.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}