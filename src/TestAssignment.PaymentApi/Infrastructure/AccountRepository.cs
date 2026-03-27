using Microsoft.EntityFrameworkCore;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Infrastructure.Persistence;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Infrastructure;

public sealed class AccountRepository(PaymentDbContext paymentDbContext) : IAccountRepository
{
    private readonly PaymentDbContext _paymentDbContext = paymentDbContext;

    public IUnitOfWork UnitOfWork => _paymentDbContext;

    public Task<Account?> GetByIdAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default)
    {
        return _paymentDbContext.Accounts
            .FirstOrDefaultAsync(
                account => account.Id == accountId,
                cancellationToken);
    }

    public Task<Account?> GetByOwnerIdAsync(
        AccountOwnerId ownerId,
        CancellationToken cancellationToken = default)
    {
        return _paymentDbContext.Accounts
            .FirstOrDefaultAsync(
                account => account.OwnerId == ownerId,
                cancellationToken);
    }

    public async Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        await _paymentDbContext.Accounts.AddAsync(account, cancellationToken);
    }
}