namespace TestAssignment.IdentityApi.Application.IntrospectAccessToken;

public sealed record IntrospectAccessTokenCommandResult(
    bool IsActive,
    Guid? UserId,
    string? Login);