using Ardalis.Result;
using MediatR;

namespace TestAssignment.IdentityApi.Application.Users.Commands.Logout;

public sealed record LogoutCommand(
    string AccessToken) : IRequest<Result<LogoutCommandResult>>;