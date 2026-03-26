namespace TestAssignment.PaymentApi.Domain.Shared;

public static class PaymentRules
{
    public static Currency Currency => Currency.Usd;

    public static Money InitialBalance => Money.UsdFromMinorUnits(800);

    public static Money ChargeAmount => Money.UsdFromMinorUnits(110);
}