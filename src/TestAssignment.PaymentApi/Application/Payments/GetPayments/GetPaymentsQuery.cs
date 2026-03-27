using MediatR;

namespace TestAssignment.PaymentApi.Application.Payments.GetPayments;

public sealed record GetPaymentsQuery(Guid OwnerId) : IRequest<IReadOnlyList<GetPaymentsItem>>;