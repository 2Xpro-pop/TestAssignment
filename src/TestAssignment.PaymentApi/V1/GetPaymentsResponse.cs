namespace TestAssignment.PaymentApi.V1;

public sealed record GetPaymentsResponse(
    Guid PaymentId,
    Guid AccountId,
    long AmountMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);