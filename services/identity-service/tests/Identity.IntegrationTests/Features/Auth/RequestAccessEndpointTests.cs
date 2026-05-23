[Collection("Identity")]
public class RequestAccessEndpointTests(IdentityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RequestAccess_ValidEmail_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/auth/request-access", new
        {
            email = "test@pncm.az",
            clientType = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RequestAccess_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/request-access", new
        {
            email = "not-an-email",
            clientType = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RequestAccess_MissingEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/auth/request-access", new
        {
            email = "",
            clientType = "Web"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}