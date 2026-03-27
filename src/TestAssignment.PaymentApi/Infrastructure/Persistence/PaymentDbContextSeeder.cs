using Microsoft.EntityFrameworkCore;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.ServiceDefaults;
using TestAssignment.ServiceDefaults.Extensions;

namespace TestAssignment.PaymentApi.Infrastructure.Persistence;

public sealed class PaymentDbContextSeeder(
    ILogger<PaymentDbContextSeeder> logger) : IDbSeeder<PaymentDbContext>
{
    private readonly ILogger<PaymentDbContextSeeder> _logger = logger;

    public async Task SeedAsync(PaymentDbContext context)
    {
        if (await context.Accounts.AnyAsync())
        {
            _logger.LogInformation("Payment database already seeded.");
            return;
        }

        var accounts = new[]
        {
            Account.Create(AccountOwnerId.Create(SeedUsers.TestUserId)),
            Account.Create(AccountOwnerId.Create(SeedUsers.AdminUserId))
        };

        await context.Accounts.AddRangeAsync(accounts);
        await context.SaveChangesAsync();

        _logger.LogInformation(
            "Payment database seeded with {AccountsCount} accounts.",
            accounts.Length);
    }
}