using MediatR;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;

namespace TestAssignment.PaymentApi.Application.Payments.GetPayments;

public sealed class GetPaymentsQueryHandler(IPaymentRepository paymentRepository)
    : IRequestHandler<GetPaymentsQuery, IReadOnlyList<GetPaymentsItem>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;

    public async Task<IReadOnlyList<GetPaymentsItem>> Handle(
        GetPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = AccountOwnerId.Create(request.OwnerId);

        var payments = await _paymentRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        var result = payments
            .Select(payment => new GetPaymentsItem(
                PaymentId: payment.Id.Value,
                AccountId: payment.AccountId.Value,
                AmountMinorUnits: payment.Amount.MinorUnits,
                CurrencyCode: payment.Amount.Currency.Code,
                CreatedAtUtc: payment.CreatedAtUtc))
            .ToList();

        return result;
    }
}