using MediatR;

namespace TestAssignment.IdentityApi.Application.IntrospectAccessToken;

public sealed record IntrospectAccessTokenCommand(string AccessToken)
    : IRequest<IntrospectAccessTokenCommandResult>;