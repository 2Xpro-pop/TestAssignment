using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Domain.Users;

public sealed class User: AggregateRoot<UserId>
{
    // For EF Core
    private User()
    {
    }

    private User(
        UserId id,
        Login login,
        PasswordHash passwordHash,
        LockoutState lockoutState): base(id)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        LockoutState = lockoutState;
    }

    public Login Login { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public LockoutState LockoutState { get; private set; }

    public static User Create(
        Login login,
        PasswordHash passwordHash)
    {
        return new User(
            UserId.New(),
            login,
            passwordHash,
            new LockoutState(
                failedLoginCount: 0,
                lockoutEndUtc: null));
    }

    internal static User Rehydration(
        UserId id,
        Login login,
        PasswordHash passwordHash,
        LockoutState lockoutState)
    {
        return new User(
            id,
            login,
            passwordHash,
            lockoutState
        );
    }

    public bool CanLogin(DateTimeOffset nowUtc)
    {
        return LockoutState.CanLogin(nowUtc);
    }

    public void RecordFailedLoginAttempt(
        DateTimeOffset nowUtc,
        int maxFailedLoginAttempts,
        TimeSpan lockoutDuration)
    {
        LockoutState = LockoutState.RegisterFailedLogin(
            nowUtc,
            maxFailedLoginAttempts,
            lockoutDuration);
    }

    public void RecordSuccessfulLogin()
    {
        LockoutState = LockoutState.Reset();
    }

    public void ChangePassword(PasswordHash passwordHash)
    {
        PasswordHash = passwordHash;
    }
}