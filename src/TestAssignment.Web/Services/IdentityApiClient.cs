using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestAssignment.Web.Services;

public sealed class IdentityApiClient(HttpClient httpClient)
{
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/identity/login",
            new { Login = login, Password = password },
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken);
            return LoginResult.Success(body!.AccessToken, body.Login, body.ExpiresAtUtc);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Locked)
        {
            return LoginResult.Failure("Account is locked. Please try again later.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return LoginResult.Failure("Invalid login or password.");
        }

        return LoginResult.Failure("Login failed. Please try again.");
    }

    public async Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/identity/logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        await httpClient.SendAsync(request, cancellationToken);
    }

    private sealed record LoginResponseDto(string AccessToken, DateTimeOffset ExpiresAtUtc, string Login);
}

public sealed record LoginResult(bool IsSuccess, string? ErrorMessage, string? AccessToken, string? Login, DateTimeOffset? ExpiresAtUtc)
{
    public static LoginResult Success(string accessToken, string login, DateTimeOffset expiresAtUtc)
        => new(true, null, accessToken, login, expiresAtUtc);

    public static LoginResult Failure(string errorMessage)
        => new(false, errorMessage, null, null, null);
}
