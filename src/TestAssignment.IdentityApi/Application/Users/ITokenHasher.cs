using TestAssignment.IdentityApi.Domain.Sesssions;

namespace TestAssignment.IdentityApi.Application.Users;

public interface ITokenHasher
{
    public TokenHash Hash(string token);
}