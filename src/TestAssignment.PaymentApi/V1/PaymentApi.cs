using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestAssignment.PaymentApi.Application.Payments.CreatePayment;
using TestAssignment.PaymentApi.Application.Payments.GetPayments;
using TestAssignment.PaymentApi.Domain.Accounts;
using AspResult = Microsoft.AspNetCore.Http.IResult;


namespace TestAssignment.PaymentApi.V1;

public static class PaymentApi
{
    public static RouteGroupBuilder MapPaymentApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/payment")
            .HasApiVersion(1.0)
            .WithTags("Payment")
            .RequireAuthorization();

        api.MapGet("/", GetPaymentsAsync)
            .WithName("Payment_GetAll")
            .WithSummary("Returns all payments of the authenticated user.")
            .Produces<IReadOnlyList<GetPaymentsResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        api.MapPost("/", CreatePaymentAsync)
            .WithName("Payment_Create")
            .WithSummary("Creates a payment and debits 1.10 USD from the authenticated user's account.")
            .Produces<CreatePaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict);

        return api;
    }

    private static async Task<AspResult> GetPaymentsAsync(
        ClaimsPrincipal claimsPrincipal,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerIdClaimValue =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ??
            claimsPrincipal.FindFirstValue("sub");

        if (!Guid.TryParse(ownerIdClaimValue, out var ownerId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(
            new GetPaymentsQuery(ownerId),
            cancellationToken);

        var response = result
            .Select(payment => new GetPaymentsResponse(
                PaymentId: payment.PaymentId,
                AccountId: payment.AccountId,
                AmountMinorUnits: payment.AmountMinorUnits,
                CurrencyCode: payment.CurrencyCode,
                CreatedAtUtc: payment.CreatedAtUtc))
            .ToList();

        return TypedResults.Ok(response);
    }

    private static async Task<AspResult> CreatePaymentAsync(
        ClaimsPrincipal claimsPrincipal,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerIdClaimValue =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ??
            claimsPrincipal.FindFirstValue("sub");

        if (!Guid.TryParse(ownerIdClaimValue, out var ownerId))
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            var result = await sender.Send(
                new CreatePaymentCommand(ownerId),
                cancellationToken);

            return TypedResults.Ok(new CreatePaymentResponse(
                PaymentId: result.PaymentId,
                AccountId: result.AccountId,
                DebitedMinorUnits: result.DebitedMinorUnits,
                RemainingBalanceMinorUnits: result.RemainingBalanceMinorUnits,
                CurrencyCode: result.CurrencyCode,
                CreatedAtUtc: result.CreatedAtUtc));
        }
        catch (InsufficientFundsException exception)
        {
            return TypedResults.BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return TypedResults.Conflict(new
            {
                message = "The account was changed by another request. Please retry."
            });
        }
        catch (ArgumentException exception)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["payment"] = [exception.Message]
            });
        }
    }
}