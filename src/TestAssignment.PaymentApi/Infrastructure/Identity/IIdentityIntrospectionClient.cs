namespace TestAssignment.PaymentApi.Infrastructure.Identity;

public interface IIdentityIntrospectionClient
{
    public Task<IdentityIntrospectionResponse?> IntrospectAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}