using Ardalis.Result;
using MediatR;
using TestAssignment.IdentityApi.Domain.Sesssions;

namespace TestAssignment.IdentityApi.Application.Users.Commands.Logout;

public sealed class LogoutCommandHandler(
    IUserSessionRepository userSessionRepository,
    ITokenHasher tokenHasher,
    TimeProvider timeProvider)
    : IRequestHandler<LogoutCommand, Result<LogoutCommandResult>>
{
    private readonly IUserSessionRepository _userSessionRepository = userSessionRepository;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<LogoutCommandResult>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHasher.Hash(request.AccessToken);

        var userSession = await _userSessionRepository.GetByTokenHashAsync(
            tokenHash,
            cancellationToken);

        if (userSession is null)
        {
            return Result<LogoutCommandResult>.Success(new LogoutCommandResult());
        }

        userSession.Revoke(_timeProvider.GetUtcNow());

        await _userSessionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LogoutCommandResult>.Success(new LogoutCommandResult());
    }
}