using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Application.Users;

public interface IPasswordHasher
{
    public PasswordHash HashPassword(string password);

    public bool Verify(string password, PasswordHash passwordHash);
}