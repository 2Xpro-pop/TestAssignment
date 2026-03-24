using Ardalis.Result;
using MediatR;
using TestAssignment.IdentityApi.Domain.Users;
using DomainUsers = TestAssignment.IdentityApi.Domain.Users;

namespace TestAssignment.IdentityApi.Application.Users.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRequestHandler<LoginCommand, Result<LoginCommandResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<LoginCommandResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var login = new DomainUsers.Login(request.Login);

        var user = await _userRepository.GetByLoginAsync(
            login,
            cancellationToken);

        if (user is null)
        {
            return Result<LoginCommandResult>.Unauthorized();
        }

        var nowUtc = _timeProvider.GetUtcNow();

        if (!user.CanLogin(nowUtc))
        {
            return Result<LoginCommandResult>.Forbidden();
        }

        var isPasswordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            user.RecordFailedLoginAttempt(
                nowUtc,
                maxFailedLoginAttempts: 5,
                lockoutDuration: TimeSpan.FromMinutes(5));

            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return Result<LoginCommandResult>.Unauthorized();
        }

        user.RecordSuccessfulLogin();

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginCommandResult>.Success(
            new LoginCommandResult(
                UserId: user.Id,
                Login: user.Login.Value));
    }
}