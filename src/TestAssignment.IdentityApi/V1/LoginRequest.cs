namespace TestAssignment.IdentityApi.V1;

public sealed record LoginRequest(
    string Login,
    string Password);