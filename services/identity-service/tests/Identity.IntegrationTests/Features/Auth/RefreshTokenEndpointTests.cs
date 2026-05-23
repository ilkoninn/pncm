[Collection("Identity")]
public class RefreshTokenEndpointTests(IdentityApiFactory factory)
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
            firstName = "İlkin",
            lastName = "Rəcəbov",
            phoneNumber = (string?)null
        });

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<CompleteRegisterResponseDto>();
        return (registerResult!.AccessToken, registerResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ValidTokens_Returns200()
    {
        var (accessToken, refreshToken) = await GetTokensAsync("refresh@pncm.az");

        var response = await _client.PostAsJsonAsync("/auth/refresh-token", new
        {
            accessToken,
            refreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_InvalidRefreshToken_Returns200WithError()
    {
        var (accessToken, _) = await GetTokensAsync("refresh2@pncm.az");

        var response = await _client.PostAsJsonAsync("/auth/refresh-token", new
        {
            accessToken,
            refreshToken = "invalidrefreshtoken"
        });

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        result!.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_MissingTokens_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/refresh-token", new
        {
            accessToken = "",
            refreshToken = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}