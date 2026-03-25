using Microsoft.EntityFrameworkCore;
using TestAssignment.IdentityApi.Domain.Sesssions;
using TestAssignment.IdentityApi.Infrastructure.Persistence;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Infrastructure.UserSessions;

public sealed class UserSessionRepository(IdentityDbContext dbContext) : IUserSessionRepository
{
    private readonly IdentityDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<UserSession?> GetByIdAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions.SingleOrDefaultAsync(
            userSession => userSession.Id == sessionId,
            cancellationToken);
    }

    public async Task<UserSession?> GetByTokenHashAsync(
        TokenHash tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions.SingleOrDefaultAsync(
            userSession => userSession.TokenHash == tokenHash,
            cancellationToken);
    }

    public async Task AddAsync(
        UserSession userSession,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.UserSessions.AddAsync(userSession, cancellationToken);
    }
}