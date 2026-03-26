using TestAssignment.PaymentApi.Domain.Shared;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Domain.Accounts;

public sealed class Account : AggregateRoot<AccountId>
{
    // For EF Core
    private Account()
    {
    }

    private Account(
        AccountId id,
        AccountOwnerId ownerId,
        Money balance)
        : base(id)
    {
        OwnerId = ownerId;
        Balance = balance;
    }

    public AccountOwnerId OwnerId { get; private set; }

    public Money Balance { get; private set; }

    public static Account Create(AccountOwnerId ownerId)
    {
        return new(
            id: AccountId.New(),
            ownerId: ownerId,
            balance: PaymentRules.InitialBalance);
    }

    public bool CanDebit(Money amount)
    {
        return Balance.IsGreaterThanOrEqualTo(amount);
    }

    public void Debit(Money amount)
    {
        if (amount.IsZero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        if (!CanDebit(amount))
        {
            throw new InsufficientFundsException();
        }

        Balance = Balance.Subtract(amount);
    }
}