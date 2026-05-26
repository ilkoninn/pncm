[Collection("Community")]
public sealed class CreatePostEndpointTests(CommunityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreatePost_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/posts", new
        {
            userId = Guid.NewGuid(),
            petId = (Guid?)null,
            content = "Test məzmunu.",
            mediaIds = Array.Empty<Guid>()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreatePost_InvalidRequest_Returns400()
    {
        var response = await _client.PostAsync("/posts",
            new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
