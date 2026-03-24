using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestAssignment.IdentityApi.Infrastructure.Persistence;

namespace TestAssignment.IdentityApi.Infrastructure;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

        dbContextOptionsBuilder.UseNpgsql();

        return new IdentityDbContext(dbContextOptionsBuilder.Options);
    }
}