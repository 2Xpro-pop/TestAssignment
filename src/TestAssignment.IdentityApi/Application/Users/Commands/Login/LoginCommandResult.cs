using Ardalis.Result;
using MediatR;
using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Application.Users.Commands.Login;

public sealed record LoginCommandResult(
    UserId UserId,
    string Login,
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);