[Collection("Identity")]
public class LogoutEndpointTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IDatabase _redis = factory.GetRedisConnection().GetDatabase();

    private async Task<(string accessToken, string refreshToken)> GetTokensAsync(string email)
    {
        await _client.PostAsJsonAsync("/auth/request-access", new
        {
            email,
            clientType = "Web"
        });

        var server = factory.GetRedisConnection().GetServer(
            factory.GetRedisConnection().GetEndPoints()[0]);

        var magicKey = server.Keys(pattern: "magic:*").FirstOrDefault();
        var token = magicKey.ToString().Replace("magic:", "");

        await _client.GetAsync($"/auth/magic?token={token}");

        var code = await _redis.StringGetAsync($"magic-code:{email}");

        var verifyResponse = await _client.PostAsJsonAsync("/auth/verify-access", new
        {
            email,
            code = code.ToString(),
            client = "Web"
        });

        var verifyResult = await verifyResponse.Content.ReadFromJsonAsync<VerifyAccessResponseDto>();

        var registerResponse = await _client.PostAsJsonAsync("/auth/complete-register", new
        {
            registrationToken = verifyResult!.RegistrationToken,
            firstName = "Test",
            lastName = "User",
            phoneNumber = (string?)null
        });

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<CompleteRegisterResponseDto>();
        return (registerResult!.AccessToken, registerResult.RefreshToken);
    }

    [Fact]
    public async Task Logout_ValidTokens_Returns200()
    {
        var (accessToken, refreshToken) = await GetTokensAsync("logout@pncm.az");

        var response = await _client.PostAsJsonAsync("/auth/logout", new
        {
            accessToken,
            refreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_AccessTokenBlacklisted_Returns401()
    {
        var (accessToken, refreshToken) = await GetTokensAsync("logout2@pncm.az");

        await _client.PostAsJsonAsync("/auth/logout", new
        {
            accessToken,
            refreshToken
        });

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var meResponse = await _client.GetAsync("/auth/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_MissingTokens_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/logout", new
        {
            accessToken = "",
            refreshToken = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}