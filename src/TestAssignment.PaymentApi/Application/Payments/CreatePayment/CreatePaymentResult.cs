namespace TestAssignment.PaymentApi.Application.Payments.CreatePayment;

public sealed record CreatePaymentResult(
    Guid PaymentId,
    Guid AccountId,
    long DebitedMinorUnits,
    long RemainingBalanceMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);