[Collection("Pet")]
public sealed class GetPetByIdEndpointTests(PetApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPetById_ExistingId_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/pets", new
        {
            name = "Rex",
            species = 0,
            breed = (string?)null,
            ageMonths = (int?)null,
            gender = 0,
            size = 0,
            color = (string?)null,
            description = (string?)null,
            isVaccinated = false,
            isNeutered = false,
            ownerId = Guid.NewGuid(),
            ownerType = 0,
            city = "Bakı",
            latitude = (decimal?)null,
            longitude = (decimal?)null
        });

        var created = await createResponse.Content.ReadFromJsonAsync<PetResponseDto>();

        var response = await _client.GetAsync($"/pets/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPetById_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/pets/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
