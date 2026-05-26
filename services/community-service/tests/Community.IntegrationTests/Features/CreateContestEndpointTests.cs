[Collection("Community")]
public sealed class CreateContestEndpointTests(CommunityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateContest_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/contests", new
        {
            title = "Test Yarışması",
            description = "Yarışma açıqlaması.",
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddDays(7),
            prize = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateContest_InvalidRequest_Returns400()
    {
        var response = await _client.PostAsync("/contests",
            new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
