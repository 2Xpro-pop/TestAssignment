using MediatR;

namespace TestAssignment.PaymentApi.Application.Payments.GetBalance;

public sealed record GetBalanceQuery(Guid OwnerId) : IRequest<GetBalanceResult>;