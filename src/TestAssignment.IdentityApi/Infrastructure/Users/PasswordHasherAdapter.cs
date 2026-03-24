using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Infrastructure.Users;

public sealed class PasswordHasherAdapter(
    IOptions<PasswordHasherOptions> passwordHasherOptions)
    : IPasswordHasher
{
    private static readonly object PasswordHasherUser = new();

    private readonly PasswordHasher<object> _passwordHasher = new(passwordHasherOptions);

    public PasswordHash HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hashedPassword = _passwordHasher.HashPassword(
            PasswordHasherUser,
            password);

        return new PasswordHash(hashedPassword);
    }

    public bool Verify(string password, PasswordHash passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            PasswordHasherUser,
            passwordHash.Value,
            password);

        return passwordVerificationResult is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}