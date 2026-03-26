using MediatR;
using TestAssignment.PaymentApi.Domain.Accounts;
using TestAssignment.PaymentApi.Domain.Payments;
using TestAssignment.PaymentApi.Domain.Shared;
using TestAssignment.ServiceDefaults;

namespace TestAssignment.PaymentApi.Application.Payments.CreatePayment;

public sealed class CreatePaymentCommandHandler(
    IAccountRepository accountRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreatePaymentCommand, CreatePaymentResult>
{
    private static readonly Money PaymentAmount = Money.Usd(1.10m);

    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<CreatePaymentResult> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId = AccountOwnerId.Create(request.OwnerId);

        var account = await _accountRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (account is null)
        {
            account = Account.Create(ownerId);
            await _accountRepository.AddAsync(account, cancellationToken);
        }

        account.Debit(PaymentAmount);

        var payment = Payment.Create(
            accountId: account.Id,
            ownerId: ownerId,
            amount: PaymentAmount,
            createdAtUtc: _timeProvider.GetUtcNow());

        await _paymentRepository.AddAsync(payment, cancellationToken);

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CreatePaymentResult(
            PaymentId: payment.Id.Value,
            AccountId: account.Id.Value,
            DebitedMinorUnits: payment.Amount.MinorUnits,
            RemainingBalanceMinorUnits: account.Balance.MinorUnits,
            CurrencyCode: account.Balance.Currency.Code,
            CreatedAtUtc: payment.CreatedAtUtc);
    }
}