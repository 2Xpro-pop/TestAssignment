using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using TestAssignment.PaymentApi.Infrastructure.Identity;

namespace TestAssignment.PaymentApi.Infrastructure.Authentication;

public sealed class IdentityIntrospectionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder urlEncoder,
    IIdentityIntrospectionClient identityIntrospectionClient)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, urlEncoder)
{
    private readonly IIdentityIntrospectionClient _identityIntrospectionClient = identityIntrospectionClient;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = TryReadBearerToken(Request);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return AuthenticateResult.NoResult();
        }

        IdentityIntrospectionResponse? identityIntrospectionResponse;

        try
        {
            identityIntrospectionResponse = await _identityIntrospectionClient.IntrospectAsync(
                accessToken,
                Context.RequestAborted);
        }
        catch
        {
            return AuthenticateResult.Fail("Identity introspection failed.");
        }

        if (identityIntrospectionResponse is null ||
            !identityIntrospectionResponse.IsActive ||
            identityIntrospectionResponse.UserId is null)
        {
            return AuthenticateResult.Fail("Invalid access token.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, identityIntrospectionResponse.UserId.Value.ToString()),
            new("sub", identityIntrospectionResponse.UserId.Value.ToString())
        };

        if (!string.IsNullOrWhiteSpace(identityIntrospectionResponse.Login))
        {
            claims.Add(new Claim(ClaimTypes.Name, identityIntrospectionResponse.Login));
        }

        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return AuthenticateResult.Success(authenticationTicket);
    }

    private static string? TryReadBearerToken(HttpRequest request)
    {
        if (!request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaderValues))
        {
            return null;
        }

        var authorizationHeader = authorizationHeaderValues.ToString();

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = authorizationHeader["Bearer ".Length..].Trim();

        return string.IsNullOrWhiteSpace(token)
            ? null
            : token;
    }
}