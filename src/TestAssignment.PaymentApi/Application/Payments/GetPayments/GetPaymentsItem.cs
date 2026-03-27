namespace TestAssignment.PaymentApi.Application.Payments.GetPayments;

public sealed record GetPaymentsItem(
    Guid PaymentId,
    Guid AccountId,
    long AmountMinorUnits,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc);