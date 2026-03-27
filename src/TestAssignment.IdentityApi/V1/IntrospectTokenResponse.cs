namespace TestAssignment.IdentityApi.V1;

public sealed record IntrospectTokenResponse(
    bool IsActive,
    Guid? UserId,
    string? Login);