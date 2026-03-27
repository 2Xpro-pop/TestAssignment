namespace TestAssignment.Web.Services;

public sealed class SessionState
{
    public string? AccessToken { get; private set; }

    public string? Login { get; private set; }

    public DateTimeOffset? ExpiresAtUtc { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && ExpiresAtUtc > DateTimeOffset.UtcNow;

    public event Action? OnChange;

    public void SetSession(string accessToken, string login, DateTimeOffset expiresAtUtc)
    {
        AccessToken = accessToken;
        Login = login;
        ExpiresAtUtc = expiresAtUtc;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        AccessToken = null;
        Login = null;
        ExpiresAtUtc = null;
        OnChange?.Invoke();
    }
}
