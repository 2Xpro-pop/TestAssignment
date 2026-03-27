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

    private sealed record ErrorDto(string? Message);
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
