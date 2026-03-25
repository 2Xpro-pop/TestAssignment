using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using TestAssignment.IdentityApi.Application.Users;

namespace TestAssignment.IdentityApi.Infrastructure.UserSessions;


public sealed class TokenGenerator : ITokenGenerator
{
    public string Generate()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(tokenBytes);
    }
}