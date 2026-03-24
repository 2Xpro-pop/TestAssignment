namespace TestAssignment.IdentityApi.Domain.Users;

public readonly record struct Login
{
    public Login(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Login cannot be null or whitespace.", nameof(value));
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length is < 3 or > 64)
        {
            throw new ArgumentException("Login length must be between 3 and 64 characters.", nameof(value));
        }

        Value = normalizedValue;
    }

    public string Value { get; }

    public override string ToString() => Value;
}