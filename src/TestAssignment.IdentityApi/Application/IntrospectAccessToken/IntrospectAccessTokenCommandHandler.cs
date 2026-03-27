using MediatR;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Domain.Sesssions;
using TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Application.IntrospectAccessToken;

public sealed class IntrospectAccessTokenCommandHandler(
    IUserSessionRepository userSessionRepository,
    IUserRepository userRepository,
    ITokenHasher accessTokenHasher,
    TimeProvider timeProvider)
    : IRequestHandler<IntrospectAccessTokenCommand, IntrospectAccessTokenCommandResult>
{
    private readonly IUserSessionRepository _userSessionRepository = userSessionRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenHasher _accessTokenHasher = accessTokenHasher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<IntrospectAccessTokenCommandResult> Handle(
        IntrospectAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken))
        {
            return new IntrospectAccessTokenCommandResult(
                IsActive: false,
                UserId: null,
                Login: null);
        }

        var accessTokenHash = _accessTokenHasher.Hash(request.AccessToken);

        var userSession = await _userSessionRepository.GetByTokenHashAsync(
            accessTokenHash,
            cancellationToken);

        var utcNow = _timeProvider.GetUtcNow();

        if (userSession is null)
        {
            return new IntrospectAccessTokenCommandResult(
                IsActive: false,
                UserId: null,
                Login: null);
        }

        if(!userSession.IsActive(utcNow))
        {
            return new IntrospectAccessTokenCommandResult(
                IsActive: false,
                UserId: null,
                Login: null);
        }

        var user = await _userRepository.GetByIdAsync(
            userSession.UserId,
            cancellationToken);

        if (user is null)
        {
            return new IntrospectAccessTokenCommandResult(
                IsActive: false,
                UserId: null,
                Login: null);
        }

        return new IntrospectAccessTokenCommandResult(
            IsActive: true,
            UserId: user.Id.Value,
            Login: user.Login.Value);
    }
}