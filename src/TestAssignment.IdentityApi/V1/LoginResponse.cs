namespace TestAssignment.IdentityApi.V1;

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string Login);