[Collection("Adoption")]
public sealed class GetAdoptionByIdEndpointTests(AdoptionApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAdoptionById_ExistingId_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/adoptions", new
        {
            petId = Guid.NewGuid(),
            adopterId = Guid.NewGuid(),
            message = "Test müraciəti.",
            contactPhone = "+994501234567"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<AdoptionResponseDto>();

        var response = await _client.GetAsync($"/adoptions/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAdoptionById_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/adoptions/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
