using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Domain.Users;

public interface IUserRepository: IRepository<User>
{
    public ValueTask<User?> GetByIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    public ValueTask<User?> GetByLoginAsync(
        Login login,
        CancellationToken cancellationToken = default);

    public ValueTask<bool> ExistsByLoginAsync(
        Login login,
        CancellationToken cancellationToken = default);

    public ValueTask AddAsync(
        User user,
        CancellationToken cancellationToken = default);
}