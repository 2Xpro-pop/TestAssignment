using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestAssignment.IdentityApi.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

        dbContextOptionsBuilder.UseNpgsql();

        return new IdentityDbContext(dbContextOptionsBuilder.Options);
    }
}