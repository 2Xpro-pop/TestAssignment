using MediatR;
using TestAssignment.PaymentApi.Domain.Accounts;

namespace TestAssignment.PaymentApi.Application.Payments.GetBalance;

public sealed class GetBalanceQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetBalanceQuery, GetBalanceResult>
{
    private readonly IAccountRepository _accountRepository = accountRepository;

    public async Task<GetBalanceResult> Handle(
        GetBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = AccountOwnerId.Create(request.OwnerId);

        var account = await _accountRepository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException("Payment account was not provisioned for the user.");
        }

        return new GetBalanceResult(
            AccountId: account.Id.Value,
            BalanceMinorUnits: account.Balance.MinorUnits,
            CurrencyCode: account.Balance.Currency.Code);
    }
}