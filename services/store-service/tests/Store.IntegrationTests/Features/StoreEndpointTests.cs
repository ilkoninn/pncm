[Collection("Store")]
public sealed class StoreEndpointTests(StoreApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateStore_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/stores", new
        {
            name = "PetShop Bakı",
            address = "Nizami küç. 10",
            city = "Bakı",
            latitude = 40.4093,
            longitude = 49.8671,
            description = "Test mağaza",
            logoUrl = (string?)null,
            phoneNumber = "+994121234567",
            ownerId = Guid.NewGuid()
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetAllStores_Returns200()
    {
        var response = await _client.GetAsync("/stores");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreById_ExistingId_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/stores", new
        {
            name = "Test Mağaza",
            address = "Test ünvan",
            city = "Bakı",
            latitude = 40.0,
            longitude = 49.0,
            description = (string?)null,
            logoUrl = (string?)null,
            phoneNumber = (string?)null,
            ownerId = Guid.NewGuid()
        });

        var created = await createResponse.Content.ReadFromJsonAsync<StoreResponseDto>();

        var response = await _client.GetAsync($"/stores/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreById_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/stores/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStore_ExistingId_Returns204()
    {
        var createResponse = await _client.PostAsJsonAsync("/stores", new
        {
            name = "Silinəcək Mağaza",
            address = "Test ünvan",
            city = "Bakı",
            latitude = 40.0,
            longitude = 49.0,
            description = (string?)null,
            logoUrl = (string?)null,
            phoneNumber = (string?)null,
            ownerId = Guid.NewGuid()
        });

        var created = await createResponse.Content.ReadFromJsonAsync<StoreResponseDto>();

        var response = await _client.DeleteAsync($"/stores/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetNearbyStores_Returns200()
    {
        var response = await _client.GetAsync("/stores/nearby?lat=40.4093&lng=49.8671&radiusKm=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}