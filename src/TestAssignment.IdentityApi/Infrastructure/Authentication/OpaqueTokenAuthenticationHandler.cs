using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TestAssignment.IdentityApi.Application.Users;
using TestAssignment.IdentityApi.Domain.Sesssions;

namespace TestAssignment.IdentityApi.Infrastructure.Authentication;

public sealed class OpaqueTokenAuthenticationHandler(
    IOptionsMonitor<OpaqueTokenAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<OpaqueTokenAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        if (!AuthenticationHeaderValue.TryParse(authorizationHeaderValues.ToString(), out var authorizationHeader))
        {
            return AuthenticateResult.Fail("Invalid Authorization header.");
        }

        if (!string.Equals(authorizationHeader.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var accessToken = authorizationHeader.Parameter;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return AuthenticateResult.Fail("Missing bearer token.");
        }

        var tokenHasher = Context.RequestServices.GetRequiredService<ITokenHasher>();
        var userSessionRepository = Context.RequestServices.GetRequiredService<IUserSessionRepository>();
        var timeProvider = Context.RequestServices.GetRequiredService<TimeProvider>();

        var tokenHash = tokenHasher.Hash(accessToken);

        var userSession = await userSessionRepository.GetByTokenHashAsync(
            tokenHash,
            Context.RequestAborted);

        if (userSession is null)
        {
            return AuthenticateResult.Fail("Invalid access token.");
        }

        var nowUtc = timeProvider.GetUtcNow();

        if (!userSession.IsActive(nowUtc))
        {
            return AuthenticateResult.Fail("Access token is expired or revoked.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userSession.UserId.Value.ToString()),
            new("session_id", userSession.Id.Value.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            Scheme.Name);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationTicket = new AuthenticationTicket(
            claimsPrincipal,
            Scheme.Name);

        return AuthenticateResult.Success(authenticationTicket);
    }
}