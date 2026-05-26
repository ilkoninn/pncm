[Collection("Pet")]
public sealed class CreatePetEndpointTests(PetApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreatePet_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/pets", new
        {
            name = "Buddy",
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreatePet_InvalidRequest_Returns400()
    {
        var response = await _client.PostAsync("/pets",
            new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
