[Collection("Identity")]
public class CompleteRegisterEndpointTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IDatabase _redis = factory.GetRedisConnection().GetDatabase();

    private async Task<string> GetRegistrationTokenAsync(string email)
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

        var result = await verifyResponse.Content.ReadFromJsonAsync<VerifyAccessResponseDto>();
        return result!.RegistrationToken!;
    }

    [Fact]
    public async Task CompleteRegister_ValidToken_Returns200()
    {
        var registrationToken = await GetRegistrationTokenAsync("register@pncm.az");

        var response = await _client.PostAsJsonAsync("/auth/complete-register", new
        {
            registrationToken,
            firstName = "İlkin",
            lastName = "Rəcəbov",
            phoneNumber = "+994701234567"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CompleteRegister_InvalidToken_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/complete-register", new
        {
            registrationToken = "invalidtoken",
            firstName = "İlkin",
            lastName = "Rəcəbov",
            phoneNumber = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteRegister_MissingFirstName_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/complete-register", new
        {
            registrationToken = "sometoken",
            firstName = "",
            lastName = "Rəcəbov",
            phoneNumber = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}