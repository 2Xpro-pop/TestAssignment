using Microsoft.EntityFrameworkCore;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;
using TestAssignment.PaymentApi.Infrastructure.Persistence;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Infrastructure;

public sealed class PaymentRepository(PaymentDbContext paymentDbContext) : IPaymentRepository
{
    private readonly PaymentDbContext _paymentDbContext = paymentDbContext;

    public IUnitOfWork UnitOfWork => _paymentDbContext;

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        await _paymentDbContext.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByOwnerIdAsync(
        AccountOwnerId ownerId,
        CancellationToken cancellationToken = default)
    {
        var payments = await _paymentDbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.OwnerId == ownerId)
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return payments;
    }
}