using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Shared;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Domain.Payments;

public sealed class Payment : AggregateRoot<PaymentId>
{
    // For EF Core
    private Payment()
    {
    }

    private Payment(
        PaymentId id,
        AccountId accountId,
        AccountOwnerId ownerId,
        Money amount,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        AccountId = accountId;
        OwnerId = ownerId;
        Amount = amount;
        CreatedAtUtc = createdAtUtc;
    }

    public AccountId AccountId { get; private set; }

    public AccountOwnerId OwnerId { get; private set; }

    public Money Amount { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static Payment Create(
        AccountId accountId,
        AccountOwnerId ownerId,
        Money amount,
        DateTimeOffset createdAtUtc)
    {
        if (amount.IsZero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        return new(
            id: PaymentId.New(),
            accountId: accountId,
            ownerId: ownerId,
            amount: amount,
            createdAtUtc: createdAtUtc);
    }
}