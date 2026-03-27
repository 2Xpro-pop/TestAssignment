namespace TestAssignment.PaymentApi.Application.Payments.GetBalance;

public sealed record GetBalanceResult(
    Guid AccountId,
    long BalanceMinorUnits,
    string CurrencyCode);