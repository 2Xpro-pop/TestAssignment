using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Domain.Sesssions;

public interface IUserSessionRepository : IRepository<UserSession>
{
    public Task<UserSession?> GetByIdAsync(
        SessionId sessionId,
        CancellationToken cancellationToken = default);

    public Task<UserSession?> GetByTokenHashAsync(
        TokenHash tokenHash,
        CancellationToken cancellationToken = default);

    public Task AddAsync(
        UserSession userSession,
        CancellationToken cancellationToken = default);
}