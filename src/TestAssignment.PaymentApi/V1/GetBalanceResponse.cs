namespace TestAssignment.PaymentApi.V1;

public sealed record GetBalanceResponse(
    Guid AccountId,
    long BalanceMinorUnits,
    string CurrencyCode);