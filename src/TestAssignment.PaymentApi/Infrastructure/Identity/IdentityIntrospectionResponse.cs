namespace TestAssignment.PaymentApi.Infrastructure.Identity;

public sealed record IdentityIntrospectionResponse(
    bool IsActive,
    Guid? UserId,
    string? Login);