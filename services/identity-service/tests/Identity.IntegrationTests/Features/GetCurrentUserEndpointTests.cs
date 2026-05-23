[Collection("Identity")]
public class GetCurrentUserEndpointTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IDatabase _redis = factory.GetRedisConnection().GetDatabase();

    private async Task<string> GetAccessTokenAsync(string email)
    {
        await _client.PostAsJsonAsync("/auth/request-access", new
        {
            email,
            clientType = "Web"
        });

        var server = factory.GetRedisConnection().GetServer(
            factory.GetRedisConnection().GetEndPoints()[0]);

        var magicKey = server.Keys(pattern: "magic:*")
            .FirstOrDefault(k =>
            {
                var val = _redis.StringGet(k.ToString());
                return val == email;
            });

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
        return registerResult!.AccessToken;
    }

    [Fact]
    public async Task GetCurrentUser_ValidToken_Returns200()
    {
        var accessToken = await GetAccessTokenAsync("me@pncm.az");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _client.GetAsync("/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCurrentUser_NoToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}