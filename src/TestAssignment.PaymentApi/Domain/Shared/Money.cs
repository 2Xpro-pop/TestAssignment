namespace TestAssignment.PaymentApi.Domain.Shared;

public readonly record struct Money
{
    public Money(long minorUnits, Currency currency)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minorUnits);

        MinorUnits = minorUnits;
        Currency = currency;
    }

    public long MinorUnits { get; }

    public Currency Currency { get; }

    public decimal Amount => MinorUnits / 100m;

    public bool IsZero => MinorUnits == 0;

    public static Money ZeroUsd => new(0, Currency.Usd);

    public static Money Zero(Currency currency)
    {
        return new(0, currency);
    }

    public static Money FromMinorUnits(long minorUnits, Currency currency)
    {
        return new(minorUnits, currency);
    }

    public static Money FromDecimal(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Money amount cannot be negative.");
        }

        if (GetScale(amount) > 2)
        {
            throw new ArgumentException("Money amount cannot contain more than 2 decimal places.", nameof(amount));
        }

        var minorUnits = decimal.ToInt64(amount * 100m);
        return new(minorUnits, currency);
    }

    public static Money Usd(decimal amount)
    {
        return FromDecimal(amount, Currency.Usd);
    }

    public static Money UsdFromMinorUnits(long minorUnits)
    {
        return new(minorUnits, Currency.Usd);
    }

    public bool IsGreaterThanOrEqualTo(Money other)
    {
        EnsureSameCurrency(other);
        return MinorUnits >= other.MinorUnits;
    }

    public bool IsLessThanOrEqualTo(Money other)
    {
        EnsureSameCurrency(other);
        return MinorUnits <= other.MinorUnits;
    }

    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return MinorUnits > other.MinorUnits;
    }

    public bool IsLessThan(Money other)
    {
        EnsureSameCurrency(other);
        return MinorUnits < other.MinorUnits;
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(checked(MinorUnits + other.MinorUnits), Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (MinorUnits < other.MinorUnits)
        {
            throw new InvalidOperationException("Cannot subtract a greater amount from money.");
        }

        return new Money(checked(MinorUnits - other.MinorUnits), Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Money currencies must match.");
        }
    }

    private static int GetScale(decimal amount)
    {
        return (decimal.GetBits(amount)[3] >> 16) & 0xFF;
    }

    public override string ToString()
    {
        return $"{Amount:0.00} {Currency}";
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.IsGreaterThanOrEqualTo(right);
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.IsLessThanOrEqualTo(right);
    }

    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    public static bool operator >(Money left, Money right)
    {
        return left.IsGreaterThan(right);
    }

    public static bool operator <(Money left, Money right)
    {
        return left.IsLessThan(right);
    }
}