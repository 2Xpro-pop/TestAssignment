using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestAssignment.PaymentApi.Infrastructure.Persistence;

public sealed class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();

        dbContextOptionsBuilder.UseNpgsql();

        return new PaymentDbContext(dbContextOptionsBuilder.Options);
    }
}
