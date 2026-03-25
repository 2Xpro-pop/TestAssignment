using TestAssignment.IdentityApi.Domain.Users;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.IdentityApi.Domain.Sesssions;

public sealed class UserSession : AggregateRoot<SessionId>
{
    // For EF Core
    private UserSession()
    {
    }

    private UserSession(
        SessionId id,
        UserId userId,
        TokenHash tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
        : base(id)
    {
        if (expiresAtUtc <= createdAtUtc)
        {
            throw new ArgumentException(
                "Expiration time must be greater than creation time.",
                nameof(expiresAtUtc));
        }

        UserId = userId;
        TokenHash = tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public UserId UserId { get; private set; }

    public TokenHash TokenHash { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public static UserSession Create(
        UserId userId,
        TokenHash tokenHash,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return new(
            id: SessionId.New(),
            userId: userId,
            tokenHash: tokenHash,
            createdAtUtc: createdAtUtc,
            expiresAtUtc: expiresAtUtc);
    }

    public bool IsActive(DateTimeOffset nowUtc)
    {
        return RevokedAtUtc is null
            && ExpiresAtUtc > nowUtc;
    }

    public bool IsExpired(DateTimeOffset nowUtc)
    {
        return ExpiresAtUtc <= nowUtc;
    }

    public bool IsRevoked()
    {
        return RevokedAtUtc is not null;
    }

    public void Revoke(DateTimeOffset nowUtc)
    {
        if (RevokedAtUtc is not null)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(nowUtc, CreatedAtUtc);

        RevokedAtUtc = nowUtc;
    }
}