using Ardalis.Result;
using MediatR;

namespace TestAssignment.IdentityApi.Application.Users.Commands.Login;

public sealed record LoginCommand(
    string Login,
    string Password) : IRequest<Result<LoginCommandResult>>;