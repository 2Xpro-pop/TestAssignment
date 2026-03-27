using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestAssignment.Web.Services;

public sealed class PaymentApiClient(HttpClient httpClient)
{
    public async Task<PaymentResult> CreatePaymentAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/payment");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<PaymentResponseDto>(cancellationToken);
            return PaymentResult.Success(body!);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return PaymentResult.Failure("Session expired. Please log in again.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorDto>(cancellationToken);
            return PaymentResult.Failure(error?.Message ?? "Insufficient funds.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return PaymentResult.Failure("Concurrent update. Please retry.");
        }

        return PaymentResult.Failure("Payment failed. Please try again.");
    }

    public async Task<GetPaymentsResult> GetPaymentsAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/payment");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var items = await response.Content.ReadFromJsonAsync<List<PaymentItemDto>>(cancellationToken);
            return GetPaymentsResult.Success(items ?? []);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return GetPaymentsResult.Failure("Session expired. Please log in again.");
        }

        return GetPaymentsResult.Failure("Failed to load payments.");
    }

    public async Task<GetBalanceResult> GetBalanceAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/payment/balance");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<BalanceDto>(cancellationToken);
            return GetBalanceResult.Success(body!);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return GetBalanceResult.Failure("Session expired. Please log in again.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return GetBalanceResult.Failure("Account not found.");
        }

        return GetBalanceResult.Failure("Failed to load balance.");
    }

    private sealed record ErrorDto(string? Message);
}

public sealed record BalanceDto(
    Guid AccountId,
    long BalanceMinorUnits,
    string CurrencyCode);

public sealed record GetBalanceResult(bool IsSuccess, string? ErrorMessage, BalanceDto? Balance)
{
    public static GetBalanceResult Success(BalanceDto balance) => new(true, null, balance);
    public static GetBalanceResult Failure(string errorMessage) => new(false, errorMessage, null);
}

public sealed record PaymentItemDto(
    Guid PaymentId,
    Guid AccountId,
    long AmountMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);

public sealed record GetPaymentsResult(bool IsSuccess, string? ErrorMessage, IReadOnlyList<PaymentItemDto>? Payments)
{
    public static GetPaymentsResult Success(IReadOnlyList<PaymentItemDto> payments) => new(true, null, payments);
    public static GetPaymentsResult Failure(string errorMessage) => new(false, errorMessage, null);
}

public sealed record PaymentResponseDto(
    Guid PaymentId,
    Guid AccountId,
    long DebitedMinorUnits,
    long RemainingBalanceMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);

public sealed record PaymentResult(bool IsSuccess, string? ErrorMessage, PaymentResponseDto? Payment)
{
    public static PaymentResult Success(PaymentResponseDto payment) => new(true, null, payment);
    public static PaymentResult Failure(string errorMessage) => new(false, errorMessage, null);
}
