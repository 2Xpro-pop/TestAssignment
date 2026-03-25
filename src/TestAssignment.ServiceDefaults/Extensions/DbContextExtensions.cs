using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestAssignment.ServiceDefaults.Extensions;

public static class DbContextExtensions
{
    public static void AddDefaultDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(optionsAction);
        services.AddMigration<TContext>();
    }

    public static void AddDefaultDbContext<TContext, TSeeder>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
        where TSeeder : class, IDbSeeder<TContext>
    {
        services.AddDbContext<TContext>(optionsAction);
        services.AddMigration<TContext, TSeeder>();
    }
}