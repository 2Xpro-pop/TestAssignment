namespace TestAssignment.PaymentApi.Infrastructure.Identity;

public sealed class IdentityIntrospectionClient(HttpClient httpClient) : IIdentityIntrospectionClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IdentityIntrospectionResponse?> IntrospectAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var httpResponseMessage = await _httpClient.PostAsJsonAsync(
            "api/identity/internal/introspect",
            new IdentityIntrospectionRequest(accessToken),
            cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return await httpResponseMessage.Content.ReadFromJsonAsync<IdentityIntrospectionResponse>(
            cancellationToken: cancellationToken);
    }
}