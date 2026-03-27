namespace TestAssignment.PaymentApi.V1;

public sealed record CreatePaymentResponse(
    Guid PaymentId,
    Guid AccountId,
    long DebitedMinorUnits,
    long RemainingBalanceMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);