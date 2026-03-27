using Ardalis.Result;
using MediatR;
using Microsoft.Net.Http.Headers;
using TestAssignment.IdentityApi.Application.IntrospectAccessToken;
using TestAssignment.IdentityApi.Application.Users.Commands.Login;
using TestAssignment.IdentityApi.Application.Users.Commands.Logout;
using AspResult = Microsoft.AspNetCore.Http.IResult;

namespace TestAssignment.IdentityApi.V1;

public static class IdentityApi
{
    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/identity")
            .HasApiVersion(1.0)
            .WithTags("Identity");

        api.MapPost("/login", LoginAsync)
            .WithName("Identity_Login")
            .WithSummary("Authenticates a user and creates a new session.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status423Locked);

        api.MapPost("/logout", LogoutAsync)
            .WithName("Identity_Logout")
            .WithSummary("Revokes current access token.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        api.MapPost("/internal/introspect", IntrospectAsync)
            .WithName("Identity_Introspect")
            .WithSummary("Validates an access token for internal services.")
            .Produces<IntrospectTokenResponse>(StatusCodes.Status200OK);


        return api;
    }

    private static async Task<AspResult> LoginAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["login"] = ["Login is required."]
            });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["password"] = ["Password is required."]
            });
        }

        Result<LoginCommandResult> result;

        try
        {
            result = await sender.Send(
                new LoginCommand(
                    request.Login,
                    request.Password),
                cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["login"] = [exception.Message]
            });
        }

        return result.Status switch
        {
            ResultStatus.Ok => TypedResults.Ok(new LoginResponse(
                AccessToken: result.Value.AccessToken,
                ExpiresAtUtc: result.Value.ExpiresAtUtc,
                Login: result.Value.Login)),

            ResultStatus.Unauthorized => TypedResults.Unauthorized(),

            ResultStatus.Forbidden => TypedResults.StatusCode(StatusCodes.Status423Locked),

            _ => TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected identity error.")
        };
    }

    private static async Task<AspResult> LogoutAsync(
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var accessToken = TryReadBearerToken(httpContext.Request);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(
            new LogoutCommand(accessToken),
            cancellationToken);

        return result.Status switch
        {
            ResultStatus.Ok => TypedResults.NoContent(),
            _ => TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected identity error.")
        };
    }

    private static async Task<AspResult> IntrospectAsync(
        IntrospectTokenRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new IntrospectAccessTokenCommand(request.AccessToken),
            cancellationToken);

        return TypedResults.Ok(new IntrospectTokenResponse(
            IsActive: result.IsActive,
            UserId: result.UserId,
            Login: result.Login));
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