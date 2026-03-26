using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Domain.Payments;

public interface IPaymentRepository : IRepository<Payment>
{
    public Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default);

    public Task<IReadOnlyList<Payment>> GetByOwnerIdAsync(
        AccountOwnerId ownerId,
        CancellationToken cancellationToken = default);
}