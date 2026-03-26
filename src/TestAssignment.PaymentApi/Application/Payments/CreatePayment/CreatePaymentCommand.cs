using MediatR;

namespace TestAssignment.PaymentApi.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(Guid OwnerId) : IRequest<CreatePaymentResult>;