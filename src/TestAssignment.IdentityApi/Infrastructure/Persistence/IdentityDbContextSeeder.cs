using Microsoft.EntityFrameworkCore;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Domain.Users;
using TestAssignment.ServiceDefaults.Extensions;

namespace TestAssignment.IdentityApi.Infrastructure.Persistence;

public sealed class IdentityDbContextSeeder(
        IPasswordHasher passwordHasher,
        ILogger<IdentityDbContextSeeder> logger) : IDbSeeder<IdentityDbContext>
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ILogger<IdentityDbContextSeeder> _logger = logger;

    public async Task SeedAsync(IdentityDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            _logger.LogInformation("Identity database already seeded.");
            return;
        }

        var users = new[]
        {
            User.Create(
                login: new Login("test"),
                passwordHash: _passwordHasher.HashPassword("test123")),

            User.Create(
                login: new Login("admin"),
                passwordHash: _passwordHasher.HashPassword("admin123"))
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        _logger.LogInformation(
            "Identity database seeded with {UsersCount} users.",
            users.Length);
    }
}