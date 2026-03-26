namespace TestAssignment.PaymentApi.Domain.Shared;

public readonly record struct Currency
{
    public static Currency Usd => new("USD");

    public Currency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Currency code cannot be null or whitespace.", nameof(code));
        }

        var normalizedCode = code.Trim().ToUpperInvariant();

        if (normalizedCode.Length != 3)
        {
            throw new ArgumentException("Currency code must contain exactly 3 characters.", nameof(code));
        }

        Code = normalizedCode;
    }

    public string Code { get; }

    public override string ToString()
    {
        return Code;
    }
}