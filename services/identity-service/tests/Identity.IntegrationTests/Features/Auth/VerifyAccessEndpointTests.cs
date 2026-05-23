[Collection("Identity")]
public class VerifyAccessEndpointTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly IDatabase _redis = factory.GetRedisConnection().GetDatabase();

    [Fact]
    public async Task VerifyAccess_ValidCode_Returns200()
    {
        var email = "verify_valid@pncm.az";

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

        magicKey.Should().NotBeNull();

        var token = magicKey.ToString().Replace("magic:", "");
        await _client.GetAsync($"/auth/magic?token={token}");

        var code = await _redis.StringGetAsync($"magic-code:{email}");
        code.IsNullOrEmpty.Should().BeFalse();

        var response = await _client.PostAsJsonAsync("/auth/verify-access", new
        {
            email,
            code = code.ToString(),
            client = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyAccess_InvalidCode_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/verify-access", new
        {
            email = "verify2@pncm.az",
            code = "000000",
            client = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyAccess_MissingCode_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/verify-access", new
        {
            email = "verify3@pncm.az",
            code = "",
            client = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}