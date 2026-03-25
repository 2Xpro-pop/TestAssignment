using System.Security.Cryptography;
using System.Text;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Domain.Sesssions;

namespace TestAssignment.IdentityApi.Infrastructure.UserSessions;

public sealed class TokenHasher : ITokenHasher
{
    public TokenHash Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        var hashHex = Convert.ToHexString(hashBytes);

        return new TokenHash(hashHex);
    }
}