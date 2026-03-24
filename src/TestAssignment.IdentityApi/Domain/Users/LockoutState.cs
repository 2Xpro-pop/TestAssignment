namespace TestAssignment.IdentityApi.Domain.Users;

public readonly record struct LockoutState
{
    public LockoutState(
        int failedLoginCount,
        DateTimeOffset? lockoutEndUtc)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(failedLoginCount);

        FailedLoginCount = failedLoginCount;
        LockoutEndUtc = lockoutEndUtc;
    }

    public int FailedLoginCount { get; }

    public DateTimeOffset? LockoutEndUtc { get; }

    public bool CanLogin(DateTimeOffset nowUtc)
    {
        return LockoutEndUtc is null || LockoutEndUtc <= nowUtc;
    }

    public LockoutState RegisterFailedLogin(
        DateTimeOffset nowUtc,
        int maxFailedLoginAttempts,
        TimeSpan lockoutDuration)
    {
        var nextFailedLoginCount = FailedLoginCount + 1;

        if (nextFailedLoginCount >= maxFailedLoginAttempts)
        {
            return new LockoutState(0, nowUtc.Add(lockoutDuration));
        }

        return new LockoutState(nextFailedLoginCount, LockoutEndUtc);
    }

    public LockoutState Reset()
    {
        return new LockoutState(0, null);
    }
}