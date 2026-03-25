using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TestAssignment.IdentityApi.Application.Users.Commands.Login;

namespace TestAssignment.IdentityApi.V1;

public static class IdentityApiV1
{
    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/identity")
            .HasApiVersion(1.0)
            .WithTags("Identity");

        api.MapPost("/login", LoginAsync)
            .WithName("Identity_Login")
            .WithSummary("Authenticates user by login and password.")
            .WithDescription("Returns success when credentials are valid.")
            .Produces<LoginCommandResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return api;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(
                request.Login,
                request.Password),
            cancellationToken);

        return result.ToMinimalApiResult();
    }
}
