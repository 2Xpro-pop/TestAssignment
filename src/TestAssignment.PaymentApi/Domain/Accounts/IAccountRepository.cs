using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Domain.Accounts;

public interface IAccountRepository : IRepository<Account>
{
    public Task<Account?> GetByIdAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default);

    public Task<Account?> GetByOwnerIdAsync(
        AccountOwnerId ownerId,
        CancellationToken cancellationToken = default);

    public Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default);
}