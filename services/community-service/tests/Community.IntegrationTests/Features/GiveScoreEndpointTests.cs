[Collection("Community")]
public sealed class GiveScoreEndpointTests(CommunityApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<Guid> CreateEntryAsync()
    {
        var contestResponse = await _client.PostAsJsonAsync("/contests", new
        {
            title = "Test Yarışması",
            description = "Açıqlama.",
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddDays(7),
            prize = (string?)null
        });
        var contest = await contestResponse.Content.ReadFromJsonAsync<ContestResponseDto>();

        var postResponse = await _client.PostAsJsonAsync("/posts", new
        {
            userId = Guid.NewGuid(),
            petId = (Guid?)null,
            content = "Test məzmunu.",
            mediaIds = Array.Empty<Guid>()
        });
        var post = await postResponse.Content.ReadFromJsonAsync<PostResponseDto>();

        var entryResponse = await _client.PostAsJsonAsync("/contest-entries", new
        {
            contestId = contest!.Id,
            postId = post!.Id,
            userId = Guid.NewGuid()
        });
        var entry = await entryResponse.Content.ReadFromJsonAsync<ContestEntryResponseDto>();

        return entry!.Id;
    }

    [Fact]
    public async Task GiveScore_ValidEntry_Returns200()
    {
        var entryId = await CreateEntryAsync();

        var response = await _client.PostAsJsonAsync($"/contest-entries/{entryId}/score", new
        {
            givenByUserId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GiveScore_DuplicateScore_Returns409()
    {
        var entryId = await CreateEntryAsync();
        var givenByUserId = Guid.NewGuid();

        await _client.PostAsJsonAsync($"/contest-entries/{entryId}/score", new
        {
            givenByUserId
        });

        var response = await _client.PostAsJsonAsync($"/contest-entries/{entryId}/score", new
        {
            givenByUserId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
