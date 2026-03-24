using Microsoft.EntityFrameworkCore;
using TestAssignment.IdentityApi.Domain.Users;
using TestAssignment.IdentityApi.Infrastructure.Persistence;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.Users;

public sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    private readonly IdentityDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async ValueTask<User?> GetByIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(
            user => user.Id == userId,
            cancellationToken);
    }

    public async ValueTask<User?> GetByLoginAsync(
        Login login,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(
            user => user.Login == login,
            cancellationToken);
    }

    public async ValueTask<bool> ExistsByLoginAsync(
        Login login,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(
            user => user.Login == login,
            cancellationToken);
    }

    public async ValueTask AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

}